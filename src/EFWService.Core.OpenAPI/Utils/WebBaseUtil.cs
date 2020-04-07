using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using EFWService.Core.OpenAPI.Authentication;
using EFWService.Core.OpenAPI.DynamicController;
using EFWService.Core.OpenAPI.Logger;

namespace EFWService.Core.OpenAPI.Utils
{
    internal class WebBaseUtil
    {
        private static DateTime ThisDLLCreateTime { get; set; }
        private static string ThisDLLCreateTimeStr { get; set; }

        #region .ctor
        static WebBaseUtil()
        {
            ThisDLLCreateTime = new FileInfo(typeof(WebBaseUtil).Assembly.Location).CreationTime;
        }
        #endregion

        /// <summary>
        /// 获取客户端IP地址列表
        /// </summary>
        /// <param name="HttpRequest"></param>
        /// <returns></returns>
        internal static string GetClientIPList(HttpRequest HttpRequest)
        {
            string ip = HttpRequest.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpRequest.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        /// <summary>
        /// 往HTTP头中添加OpenAPI.WebBase.DLL的创建时间
        /// </summary>
        /// <param name="HttpResponse"></param>
        internal static void CustClientInfo(HttpRequest HttpRequest, HttpResponse response)
        {
            response.Headers.Add("T", ThisDLLCreateTimeStr ?? DateTime.Now.ToString());
        }

        private static readonly Dictionary<string, ApiMethodMeta> apiMethodMetaCache = new Dictionary<string, ApiMethodMeta>();
        //接口元数据
        public static Dictionary<string, ApiMethodMeta> ApiMethodMetaCache
        {
            get
            {
                return apiMethodMetaCache;
            }
        }
        /// <summary>
        /// api日志记录器
        /// </summary>
        public static IApiLogger<ApiLogEntity> ApiLogger { get; set; }
        /// <summary>
        /// 之前后逻辑处理植入
        /// </summary>
        public static List<DefaultApiExecuteAround> ApiExecuteAroundPipeline = new List<DefaultApiExecuteAround>()
        {
            new ApiExecuteAroundForBaseInfo()
        };
        /// <summary>
        /// 认证管道列表
        /// </summary>
        public static List<Authentication.Authentication> AuthenticationPipeline = new List<Authentication.Authentication>()
        {
            new HttpMethodAuthentication(), //Http请求方法验证
        };
        /// <summary>
        /// 增加自定义认证
        /// </summary>
        /// <param name="authentication"></param>
        public static void AddAuthentication(Authentication.Authentication authentication)
        {
            AuthenticationPipeline.Add(authentication);
        }
        /// <summary>
        /// 增加自定义认证
        /// </summary>
        /// <param name="authentication"></param>
        public static void AddAuthenticationToFirst(Authentication.Authentication authentication)
        {
            List<Authentication.Authentication> au = new List<Authentication.Authentication>();
            au.Add(authentication);
            au.AddRange(AuthenticationPipeline);
            AuthenticationPipeline = au;
        }
    }
}
