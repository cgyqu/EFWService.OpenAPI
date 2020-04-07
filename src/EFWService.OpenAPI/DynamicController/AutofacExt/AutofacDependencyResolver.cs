using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;

namespace EFWService.OpenAPI.DynamicController.AutofacExt
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        public IContainer container;
        public AutofacDependencyResolver(IContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                if (scope.IsRegistered(serviceType))
                    return scope.Resolve(serviceType);
                return null;
            }
        }

        internal IEnumerable<T> GetTypeServices<T>(T serviceType)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                return scope.Resolve<IEnumerable<T>>();
            }
        }

        internal T GetServiceByKey<T>(string key)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                if (scope.IsRegisteredWithKey<T>(key))
                {
                    return scope.ResolveKeyed<T>(key);
                }
                return default(T);
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return GetTypeServices(serviceType);
        }
    }
}
