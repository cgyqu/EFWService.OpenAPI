using Microsoft.Extensions.DependencyInjection;
using EFWService.Core.OpenAPI.DynamicController;
using Microsoft.AspNetCore.Mvc;
using System;
using EFWService.Core.OpenAPI.Formater;
using Microsoft.AspNetCore.Builder;

namespace EFWService.Core.OpenAPI
{
    public static class OpenAPIExtension
    {
        public static IServiceCollection AddOpenAPI(this IServiceCollection sc)
        {
            var builder = sc.AddMvc(option =>
            {
                option.ValueProviderFactories.Clear();
                option.InputFormatters.Clear();
                option.OutputFormatters.Clear();
                option.OutputFormatters.Add(new APIOutputFormatter());
            });
            Bootstrapper.Initialize(builder);
            return sc;
        }

        public static IApplicationBuilder UseOpenAPI(this IApplicationBuilder app)
        {
            app.UseMvc();
            return app;
        }
    }
}
