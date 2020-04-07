using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EFWService.OpenAPI.Utils;

namespace EFWService.OpenAPI
{
    public class ApiExecuteAroundForBaseInfo : DefaultApiExecuteAround
    {
        public override void Before(BeforeParam param)
        {
            WebBaseUtil.CustClientInfo(param.HttpRequest, param.HttpResponse);
        }

        public override void After(AfterParam param)
        {
            param.Stopwatch.Stop();
            param.HttpResponse.AddHeader("es", param.Stopwatch.ElapsedMilliseconds.ToString());
            param.HttpResponse.AddHeader("fap", param.ApiMethodMetaInfo.Fap);
        }
    }
}
