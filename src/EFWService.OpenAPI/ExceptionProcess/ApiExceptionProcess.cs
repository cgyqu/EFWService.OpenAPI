using System;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Exceptions;
using EFWService.OpenAPI.Logger;
using EFWService.OpenAPI.Model;

namespace EFWService.OpenAPI.ExceptionProcess
{
    public class ApiExceptionProcess<RequestModelType, ResponseModelType> : IExceptionProcess<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        public string Process(Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent,
            Exception _ex, RequestModelType request, ApiLogEntity apiLogEntity)
        {
            ApiException ex = _ex as ApiException;

            string content = getErrorContent(request, new ApiResponseModelBase()
            {
                respCode = ex.ErrorCode,
                respMsg = ex.ErrorMessage
            }, _ex);
            return content;
        }
    }
}
