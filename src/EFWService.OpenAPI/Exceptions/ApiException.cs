using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.OpenAPI.Emums;

namespace EFWService.OpenAPI.Exceptions
{
    /// <summary>
    /// API错误异常类
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// 异常code初始化
        /// </summary>
        /// <param name="apiExceptionCode"></param>
        public ApiException(ApiResultCode apiExceptionCode)
        {
            this.ApiExceptionCode = apiExceptionCode;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="apiExceptionCode"></param>
        /// <param name="innerException"></param>
        public ApiException(ApiResultCode apiExceptionCode, Exception innerException)
            : base("", innerException)
        {
            this.ApiExceptionCode = apiExceptionCode;
        }
        /// <summary>
        /// 异常code
        /// </summary>
        public ApiResultCode ApiExceptionCode { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return (int)ApiExceptionCode;
            }
        }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
