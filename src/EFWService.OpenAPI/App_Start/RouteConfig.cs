using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EFWService.OpenAPI.Utils;

namespace EFWService.OpenAPI
{
    /// <summary>
    /// 路由配置
    /// </summary>
    internal class RouteConfig
    {
        /// <summary>
        /// 注册默认路由
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var controllerMeta = WebBaseUtil.ApiMethodMetaCache.Values.Select(x =>
                    new
                    {
                        Controller = x.Controller,
                        Category = x.Category,
                        Module = x.Module,
                    }).ToList().Distinct();

            foreach (var meta in controllerMeta)
            {
                routes.MapRoute(
                     name: $"{meta.Controller}_default",
                     url: $"{meta.Module}/{meta.Category}/{{action}}",
                     defaults: new { controller = meta.Controller, action = "Index", ResultType = "json" }
                );
            }

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}",
              defaults: new { controller = "Home_WelCome", action = "Index", ResultType = "json" }
          );
        }
    }
}
