using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace EFWService.OpenAPI.DynamicController.AutofacExt
{
    internal class AutofacControllerFactory : DefaultControllerFactory
    {
        static readonly string ControllerKey = "DynamicWeb_{0}";

        internal static string CreateControllerKey(string controllerName)
        {
            return string.Format(ControllerKey, controllerName).ToLower();
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            try
            {
                string key = string.Format(ControllerKey + "Controller", requestContext.RouteData.Values["controller"]).ToLower();
                var resolver = DependencyResolver.Current as AutofacDependencyResolver;
                return resolver.GetServiceByKey<IController>(key);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
