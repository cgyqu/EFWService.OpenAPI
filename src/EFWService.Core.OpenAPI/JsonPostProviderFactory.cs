using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EFWService.Core.OpenAPI
{
    public class PostDataProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("controllerContext");
            }
            var request = context.ActionContext.HttpContext.Request;
            if (!request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }
            using (var sr = new StreamReader(request.Body, Encoding.UTF8))
            {
                string str = sr.ReadToEnd();
                context.ActionContext.HttpContext.Items.Add("______jsonpost", str);
            }
            return Task.CompletedTask;
        }
    }
    /// <summary>
    /// json 兼容处理
    /// </summary>
    public static class ProviderFactoryEx
    {
        public static void Init(this IList<IValueProviderFactory> valueProviderFactories)
        {
            valueProviderFactories.Clear();
            //valueProviderFactories.Add(new PostDataProviderFactory());
            valueProviderFactories.Add(new FormValueProviderFactory());
            valueProviderFactories.Add(new QueryStringValueProviderFactory());
            valueProviderFactories.Add(new RouteValueProviderFactory());
        }
    }
}
