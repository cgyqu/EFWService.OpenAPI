using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EFWService.OpenAPI.DynamicController;
using EFWService.OpenAPI.Authentication;
using EFWService.OpenAPI.Logger;

namespace EFWService.OpenAPI.Utils
{
    /// <summary>
    /// web 基础配置类
    /// </summary>
    internal class WebBaseUtil
    {
        static WebBaseUtil()
        {
        }
        private static readonly object obj = new object();
        /// <summary>
        /// api日志记录器
        /// </summary>
        public static IApiLogger<ApiLogEntity> ApiLogger { get; set; }
        private static readonly Dictionary<string, ApiMethodMeta> apiMethodMetaCache = new Dictionary<string, ApiMethodMeta>();
        //接口元数据
        internal static Dictionary<string, ApiMethodMeta> ApiMethodMetaCache
        {
            get
            {
                return apiMethodMetaCache;
            }
        }
        /// <summary>
        /// 认证管道列表
        /// </summary>
        internal static List<Authentication.Authentication> AuthenticationPipeline = new List<Authentication.Authentication>()
        {
            new HttpMethodAuthentication(), //Http请求方法验证
        };
        /// <summary>
        /// 之前后逻辑处理植入
        /// </summary>
        internal static List<DefaultApiExecuteAround> ApiExecuteAroundPipeline = new List<DefaultApiExecuteAround>()
        {
            new ApiExecuteAroundForBaseInfo()
        };
        /// <summary>
        /// 获取客户端IP地址列表
        /// </summary>
        /// <param name="HttpRequest"></param>
        /// <returns></returns>
        internal static List<string> GetClientIPList(System.Web.HttpRequestBase HttpRequest)
        {
            string HTTP_X_FORWARDED_FOR = HttpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? "";
            string REMOTE_ADDR = HttpRequest.ServerVariables["REMOTE_ADDR"] ?? "";
            List<string> ipList = new List<string>();
            ipList.AddRange(HTTP_X_FORWARDED_FOR.Split(',').ToList());
            ipList.AddRange(REMOTE_ADDR.Split(',').ToList());
            ipList.Add(HttpRequest.UserHostAddress);
            return ipList.Distinct().Where(x => x.Length > 4).ToList();
        }
        /// <summary>
        /// 新增返回头信息
        /// </summary>
        /// <param name="HttpResponse"></param>
        internal static void CustClientInfo(System.Web.HttpRequestBase HttpRequest, System.Web.HttpResponseBase HttpResponse)
        {
            HttpResponse.AddHeader("T", DateTime.Now.ToString());
        }
    }
}
