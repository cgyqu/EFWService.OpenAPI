using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFWService.OpenAPI.Model;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Logger;

namespace EFWService.OpenAPI.ExceptionProcess
{
    public static class ErrorMsgEx
    {
        public static string NoShowTrack(this string errorMsg)
        {
            return errorMsg.Split(new string[] { ",异常跟踪ID" }, StringSplitOptions.None)[0];
        }
    }
    public class NocatchExceptionProcess<RequestModelType, ResponseModelType> : IExceptionProcess<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        public string Process(Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent, 
            Exception _ex, RequestModelType request, ApiLogEntity apiLogEntity)
        {
            string exceptionId = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            string content = getErrorContent(request, new ApiResponseModelBase()
               {
                   respCode = (int)ApiResultCode.SystemError,
                   respMsg = string.Format("未知系统错误,异常跟踪ID[{0}]", exceptionId)
               }, _ex);
            apiLogEntity.Exception = _ex;
            apiLogEntity.ExceptionId = exceptionId;
            return content;
        }


    }
}
