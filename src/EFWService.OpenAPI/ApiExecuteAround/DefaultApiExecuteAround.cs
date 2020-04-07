using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EFWService.OpenAPI.Utils;
using EFWService.OpenAPI.Logger;

namespace EFWService.OpenAPI
{
    public class DefaultApiExecuteAround
    {
        public class BeforeParam
        {
            public System.Web.HttpRequestBase HttpRequest { get; set; }
            public System.Web.HttpResponseBase HttpResponse { get; set; }
        }
        public class AfterParam
        {
            public System.Web.HttpRequestBase HttpRequest { get; set; }
            public System.Web.HttpResponseBase HttpResponse { get; set; }
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
