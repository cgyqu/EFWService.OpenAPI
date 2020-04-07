using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
               name: "MyTest_Query",
               url: "mytest/query/{action}",
               defaults: new { controller = "MyTest_Query", action = "Index", ResultType = "json" }
           );
        }
    }
}
