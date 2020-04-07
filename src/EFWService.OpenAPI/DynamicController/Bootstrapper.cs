using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using EFWService.OpenAPI.Attributes;
using EFWService.OpenAPI.DynamicController.AutofacExt;
using EFWService.OpenAPI.Model;
using EFWService.OpenAPI.Models;
using EFWService.OpenAPI.Utils;

namespace EFWService.OpenAPI.DynamicController
{
    internal static class Bootstrapper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            var list = GetControllerMeta();
            var ass = GetControllerAss(list);
            ContainerBuilder builder = new ContainerBuilder();
            foreach (var type in ass.GetTypes())
            {
                builder.RegisterType(type).Keyed<IController>(AutofacExt.AutofacControllerFactory.CreateControllerKey(type.Name));
            }
            builder.RegisterType<AutofacControllerFactory>().As<IControllerFactory>();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        /// <summary>
        /// 加载程序集合
        /// 有限api结尾的程序集，如果没有，则加载所有程序集合
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAssemblies()
        {
            IEnumerable<Assembly> assList = null;
            assList = AssemblyLocator.GetBinFolderAssemblies().Where(x => x.FullName.ToLower().Contains("api")).AsEnumerable();
            if (assList == null || assList.Count() <= 0)
            {
                assList = AssemblyLocator.GetBinFolderAssemblies().AsEnumerable();
            }
            return assList;
        }
        private static Assembly GetControllerAss(List<ApiMethodMeta> list)
        {
            CompileHelper.Init();
            List<string> codeList = new List<string>();

            foreach (var controller in list.GroupBy(x => x.Controller))
            {
                string code = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EFWService.OpenAPI.Model;
using System.Diagnostics;

namespace EFWService.OpenAPI.$TYPE$Controllers
{
    public class $C$Controller : EFWService.OpenAPI.DynamicController.BaseController
    {
    ";
                #region 构建Action
                int i = 0;
                foreach (var c in controller)
                {
                    i++;
                    if (i == 1)
                    {
                        code = code.Replace("$TYPE$", c.Module.ToString());
                    }
                    code += @" public ActionResult $Method$($RequestModel$ requestModel)
        {
            var method = new $MethodClass$();
            method.HttpRequest = base.HttpContext.Request;
            method.HttpResponse = base.HttpContext.Response;
            method.SuperContext = this;
            return method.Execute(requestModel);
        }".Replace("$MethodClass$", c.TypeName).Replace("$Method$", c.MethodName).Replace("$RequestModel$", c.IApiRequestModelType.FullName);
                }
                #endregion

                code += " }";
                code = code.Replace("$C$", controller.Key);
                code += " }";
                codeList.Add(code);
            }
            var ass = CompileHelper.CompileAssembly(codeList.ToArray());
            return ass;
        }
        /// <summary>
        /// 获取api 的相关参数
        /// </summary>
        /// <returns></returns>
        private static List<ApiMethodMeta> GetControllerMeta()
        {
            List<ApiMethodMeta> metaList = new List<ApiMethodMeta>();
            IEnumerable<Assembly> tragetAss = GetAssemblies();

            foreach (var item in tragetAss)
            {
                /*GetTypes()的作用是扫描所有api结尾dll中基类为
                 * OpenAPI.WebBase.ApiMethodBase的所有类
                 * 启动时的耗时主要在这里
                 */
                foreach (var type in GetTypes(item))
                {
                    var attrList = type.GetCustomAttributes(false);
                    if (attrList.Length > 0
                        && (attrList[0] as ApiMethodDescAttribute) != null)
                    {
                        var APIMethodDesc = attrList[0] as ApiMethodDescAttribute;

                        string nameEnd = "method";
                        if (!type.Name.ToLower().EndsWith(nameEnd))
                        {
                            continue;
                        }
                        //方法名为定义的类名
                        string methodTrueName = type.Name.Substring(0, type.Name.ToLower().IndexOf(nameEnd));
                        if (!string.IsNullOrEmpty(APIMethodDesc.MethodName))
                        {
                            methodTrueName = APIMethodDesc.MethodName;
                        }
                        var obj = new ApiMethodMeta()
                        {
                            APIMethodDesc = APIMethodDesc,
                            MethodName = methodTrueName,
                            TypeName = type.FullName,
                            MethodType = type,
                            Controller = string.Format("{0}_{1}", APIMethodDesc.Module, APIMethodDesc.Category),
                            FullTypeName = type.FullName + "," + type.Assembly.FullName.Split(',')[0]
                        };
                        obj.FullTypeNameSHA1 = SHAEncryption.Encrypt(obj.FullTypeName);
                        obj.Fap = $"{APIMethodDesc.Module}.{APIMethodDesc.Category}.{obj.MethodName}";
                        //返回泛型基类中的reqeuest类
                        foreach (var genericType in type.BaseType.GetGenericArguments())
                        {
                            var typeNow = genericType;
                            while (true)
                            {
                                if (typeNow != null && typeNow == typeof(ApiRequestModelBase))
                                {
                                    obj.IApiRequestModelType = genericType;
                                    break;
                                }
                                else
                                {
                                    if (typeNow != null)
                                    {
                                        typeNow = typeNow.BaseType;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        //确定是否需要反序列化
                        obj.IsStructuredPost = obj.IApiRequestModelType.GetInterfaces().Any(x => x == typeof(IStructuredPost));

                        lock (WebBaseUtil.ApiMethodMetaCache)
                        {

                            if (WebBaseUtil.ApiMethodMetaCache == null)
                            {
                                throw new NullReferenceException("WebBaseUtil.ApiMethodMetaCache");
                            }
                            if (obj.TypeName == null)
                            {
                                throw new NullReferenceException("obj.TypeName");
                            }
                            //关闭的接口则不再添加
                            if (!APIMethodDesc.IsClose)
                            {
                                WebBaseUtil.ApiMethodMetaCache.Add(obj.TypeName, obj);//添加到缓存
                                metaList.Add(obj);
                            }
                        }
                    }
                }
            }
            return metaList;
        }

        private static IEnumerable<Type> GetTypes(Assembly item)
        {
            try
            {
                return item.GetTypes().Where(x => BaseApiCheck(x));
            }
            catch (Exception)
            {

                return new List<Type>();
            }
        }
        /// <summary>
        ///判断基类型是否是ApiMethodBase<>
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool BaseApiCheck(Type x)
        {
            bool isApiMethodBase = x.BaseType != null && x.BaseType.FullName != null && x.BaseType.FullName.ToLower().StartsWith("EFWService.OpenAPI.ApiMethodBase`2".ToLower());
            bool isRightBaseType = false;
            try
            {
                isRightBaseType = (x.BaseType).BaseType.Name == "ApiMethodBase`2";
            }
            catch (Exception)
            {

            }
            return isApiMethodBase || isRightBaseType;
        }
    }
}
