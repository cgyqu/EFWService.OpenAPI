using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCWLService.BaseService.CommonLib;
using EFWService.OpenAPI.Model;
using System.Web;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Logger;

namespace EFWService.OpenAPI.ExceptionProcess
{
    public class HttpExceptionProcess<RequestModelType, ResponseModelType> : IExceptionProcess<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        public string Process(Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent, Exception _ex,
            RequestModelType request, ApiLogEntity apiLogEntity)
        {
            HttpException ex = _ex as HttpException;

            string exceptionId = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            string content = getErrorContent(request, new ApiResponseModelBase()
            {
                respCode = (int)ApiResultCode.HttpError,
                respMsg = string.Format("请求内部接口出错,异常跟踪ID[{0}]", exceptionId)
            }, _ex);
            apiLogEntity.Exception = ex;
            apiLogEntity.ExceptionId = exceptionId;
            return content;
        }


    }
}
