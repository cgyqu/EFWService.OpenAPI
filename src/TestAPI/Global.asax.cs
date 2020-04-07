using EFWService.OpenAPI;
using System.Web.Routing;

namespace TestAPI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            OpenAPIHelper.Init();
        }
    }
}
