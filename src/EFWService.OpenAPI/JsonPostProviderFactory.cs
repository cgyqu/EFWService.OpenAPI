using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace EFWService.OpenAPI
{
    /// <summary>
    /// 若果是application/json格式数据，会导致数据会自动序列化到实体，不能对数据做有效的验证
    /// </summary>
    public class PostDataProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            using (var sr = new StreamReader(controllerContext.HttpContext.Request.InputStream, Encoding.UTF8))
            {
                string str = sr.ReadToEnd();
                controllerContext.HttpContext.Items["______jsonpost"] = str;
            }

            return null;
        }
    }
    /// <summary>
    /// json 兼容处理
    /// </summary>
    public static class ProviderFactoryEx
    {
        public static void Init(this ValueProviderFactoryCollection valueProviderFactories)
        {
            valueProviderFactories.Clear();
            //valueProviderFactories.Add(new PostDataProviderFactory());
            valueProviderFactories.Add(new FormValueProviderFactory());
            valueProviderFactories.Add(new RouteDataValueProviderFactory());
            valueProviderFactories.Add(new QueryStringValueProviderFactory());
        }
    }
}
