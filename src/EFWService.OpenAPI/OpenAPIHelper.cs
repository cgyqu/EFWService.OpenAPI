using System.Web.Mvc;
using EFWService.OpenAPI.DynamicController;
using System.Threading;
using System.Web.Routing;

namespace EFWService.OpenAPI
{
    public class OpenAPIHelper
    {
        private static int status = 0;
        private static readonly object obj = new object();
        /// <summary>
        /// 初始化框架
        /// </summary>
        public static void Init()
        {
            if (Interlocked.Exchange(ref status, 1) != 0)
            {
                return;
            }
            lock (obj)
            {
                Bootstrapper.Initialize();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                ValueProviderFactories.Factories.Init();
                RouteConfig.RegisterRoutes(RouteTable.Routes);
            }
        }
    }
}
