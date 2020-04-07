using EFWService.Core.OpenAPI.Logger;
using EFWService.OpenAPI.Model;
using System;

namespace EFWService.Core.OpenAPI.ExceptionProcess
{
    public interface IExceptionProcess<RequestModelType, ResponseModelType>
        where RequestModelType : ApiRequestModelBase
        where ResponseModelType : ApiResponseModelBase
    {
        string Process(Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent, Exception _ex,
            RequestModelType request, ApiLogEntity apiLogEntity);
    }
}
