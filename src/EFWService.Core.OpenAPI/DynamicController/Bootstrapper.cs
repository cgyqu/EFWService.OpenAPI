using EFWService.Core.OpenAPI.Attributes;
using EFWService.Core.OpenAPI.Utils;
using EFWService.OpenAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFWService.Core.OpenAPI.DynamicController
{
    internal static class Bootstrapper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize(IMvcBuilder builder)
        {
            var list = GetControllerMeta();
            var ass = GetControllerAss(list);
            if (ass == null)
            {
                return;
            }
            //将该程序集注册
            builder.ConfigureApplicationPartManager(part =>
            {
                part.ApplicationParts.Add(new AssemblyPart(ass));
            });
            foreach (var type in ass.GetTypes())
            {
                if (typeof(Controller).IsAssignableFrom(type) && type != typeof(Controller))
                {
                    builder.Services.TryAddTransient(type, type);
                }
            }
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
                StringBuilder code = new StringBuilder();
                code.AppendFormat(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using EFWService.Core.OpenAPI.Models;
using System.Diagnostics;

namespace EFWService.Core.OpenAPI.DynamicControllers
{{
    [Route(" + $"\"api/{controller.FirstOrDefault().Module.Trim()}/{controller.FirstOrDefault().Category.Trim()}/[action]\"" + ")]" +
    @"public class {0}Controller : Controller
    {{
     ", controller.Key);
                #region 构建Action
                foreach (var c in controller)
                {
                    code.AppendFormat(@"public dynamic {0}({1} requestModel)", c.MethodName, c.IApiRequestModelType.FullName);
                    code.AppendFormat(@"
        {{
            var method = new {0}();
            method.HttpRequest = base.HttpContext.Request;
            method.HttpResponse = base.HttpContext.Response;
            method.SuperContext = this;
            return method.Execute(requestModel);
        }}", c.TypeName);
                }
                #endregion

                code.Append(" }");
                code.Append(" }");
                codeList.Add(code.ToString());
            }
            var assembly = CompileHelper.CompileAssembly(codeList.ToArray());
            return assembly;
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
                        //方法名为定义的类名
                        string methodTrueName = type.Name;
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
                            FullTypeName = type.FullName + "," + type.Assembly.FullName.Split(',')[0],
                        };
                        obj.FullTypeNameSHA1 = SHAEncryption.Encrypt(obj.FullTypeName);
                        obj.Fap = obj.Controller.Replace("_", ".") + "." + obj.MethodName;
                        //返回泛型基类中的reqeuest类
                        foreach (var btgaInt in type.BaseType.GetGenericArguments())
                        {
                            var typeNow = btgaInt;
                            for (int i = 0; i < 10; i++)
                            {
                                if (typeNow != null && typeNow == typeof(ApiRequestModelBase))
                                {
                                    obj.IApiRequestModelType = btgaInt;
                                }
                                else
                                {
                                    if (typeNow != null)
                                    {
                                        typeNow = typeNow.BaseType;
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
                            if (obj == null)
                            {
                                throw new NullReferenceException("obj");
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
            bool isApiMethodBase = x.BaseType != null && x.BaseType.FullName != null && x.BaseType.FullName.ToLower().StartsWith("EFWService.Core.OpenAPI.ApiMethodBase`2".ToLower());
            bool isRightBaseType = false;
            try
            {
                isRightBaseType = x.BaseType.Name == "ApiMethodBase`2";
            }
            catch (Exception)
            {

            }
            return isApiMethodBase || isRightBaseType;
        }
    }
}
