using EFWService.Core.OpenAPI.DynamicController;
using EFWService.Core.OpenAPI.Logger;
using EFWService.Core.OpenAPI.OutputProcessor;
using EFWService.Core.OpenAPI.Utils;
using EFWService.OpenAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace EFWService.Core.OpenAPI
{
    /// <summary>
    /// API方法接口
    /// 这个部分类放一些属性之类的相对静态部分代码
    /// </summary>
    public abstract partial class ApiMethodBase<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        private const string NoCustomOutputFun = "NoCustomOutputFun";
        private ApiLogEntity apiLogEntity = new ApiLogEntity();

        private ApiMethodMeta apiMethodMetaInfo;
        public ApiMethodMeta ApiMethodMetaInfo
        {
            get
            {
                if (apiMethodMetaInfo == null)
                {
                    apiMethodMetaInfo = WebBaseUtil.ApiMethodMetaCache[this.GetType().FullName];
                }
                return apiMethodMetaInfo;
            }
        }

        /// <summary>
        /// Http请求方法 如GET、POST
        /// </summary>
        public string HttpMethod
        {
            get
            {
                return HttpRequest.Method;
            }
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        public string ClientIPList
        {
            get
            {
                return WebBaseUtil.GetClientIPList(HttpRequest);
            }
        }

        private Stopwatch Stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// Http请求
        /// </summary>
        public HttpRequest HttpRequest { get; set; }
        /// <summary>
        /// Http响应
        /// </summary>
        public HttpResponse HttpResponse { get; set; }
        /// <summary>
        /// 超级上下文 其实就是BaseController->Controller的一切信息
        /// </summary>
        public Controller SuperContext { get; set; }

        private static Dictionary<string, IOutputProcessor> outPrDict = new Dictionary<string, IOutputProcessor>()
        {
            {"json",new JsonOutputProcessor()},
            {"xml",new XmlOutputProcessor()}
        };
    }
}
