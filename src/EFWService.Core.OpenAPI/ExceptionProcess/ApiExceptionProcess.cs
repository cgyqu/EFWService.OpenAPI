using EFWService.Core.OpenAPI.Exceptions;
using EFWService.Core.OpenAPI.Logger;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Model;
using System;

namespace EFWService.Core.OpenAPI.ExceptionProcess
{
    public class ApiExceptionProcess<RequestModelType, ResponseModelType> : IExceptionProcess<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        public string Process(Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent,
            Exception _ex, RequestModelType request, ApiLogEntity apiLogEntity)
        {
            ApiException ex = _ex as ApiException;

            var content = getErrorContent(request,new ApiResponseModelBase()
            {
                respCode = ex.ErrorCode,
                respMsg = ex.ErrorMessage
            }, _ex);
            return content;
        }
    }
}
