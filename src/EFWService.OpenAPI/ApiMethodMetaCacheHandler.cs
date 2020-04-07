using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EFWService.OpenAPI.Utils;
using TCWLService.BaseService.CommonLib;

namespace EFWService.OpenAPI
{
    public class ApiMethodMetaCacheHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            context.Response.Write(JsonConvertExd.SerializeObject(WebBaseUtil.ApiMethodMetaCache.Select(c => c.Value)));
        }
    }
}
