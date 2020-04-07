using EFWService.OpenAPI.Emums;
using System;

namespace EFWService.Core.OpenAPI.Exceptions
{
    /// <summary>
    /// API错误异常类
    /// </summary>
    internal class ApiException : Exception
    {
        public ApiException(ApiResultCode apiExceptionCode)
        {
            this.ApiExceptionCode = apiExceptionCode;
        }

        public ApiException(ApiResultCode apiExceptionCode, Exception innerException)
            : base("", innerException)
        {
            this.ApiExceptionCode = apiExceptionCode;
        }

        public string Desc { get; set; }

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
