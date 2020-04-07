using System;
using EFWService.OpenAPI.Logger;
using EFWService.OpenAPI.Model;

namespace EFWService.OpenAPI.ExceptionProcess
{
    public interface IExceptionProcess<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        string Process(Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent, Exception _ex,
            RequestModelType request, ApiLogEntity apiLogEntity);
    }
}
