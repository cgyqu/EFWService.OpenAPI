using Microsoft.AspNetCore.Http;
using EFWService.Core.OpenAPI.Logger;

namespace EFWService.Core.OpenAPI
{
    public class DefaultApiExecuteAround
    {
        public class BeforeParam
        {
            public HttpRequest HttpRequest { get; set; }
            public HttpResponse HttpResponse { get; set; }
        }
        public class AfterParam
        {
            public HttpRequest HttpRequest { get; set; }
            public HttpResponse HttpResponse { get; set; }
            public System.Diagnostics.Stopwatch Stopwatch { get; set; }

            public DynamicController.ApiMethodMeta ApiMethodMetaInfo { get; set; }

            public bool HasError { get; set; }

            public LogType LogType { get; set; }
        }

        public virtual void Before(BeforeParam param)
        {
        }

        public virtual void After(AfterParam param)
        {
        }
    }
}
