using EFWService.Core.OpenAPI.DynamicController;
using EFWService.Core.OpenAPI.Exceptions;
using EFWService.Core.OpenAPI.Logger;
using EFWService.OpenAPI.Model;
using System;

namespace EFWService.Core.OpenAPI.ExceptionProcess
{
    internal class ErrorHandler
    {
        internal static string Process<RequestModelType, ResponseModelType>(
            Func<RequestModelType, ApiResponseModelBase, Exception, string> getErrorContent, Exception ex, RequestModelType request,
            ApiLogEntity apiLogEntity,
            ApiMethodMeta apiMethodMeta
            )
            where RequestModelType : ApiRequestModelBase
            where ResponseModelType : ApiResponseModelBase
        {
            object exExd = string.Empty;
            string content = null;
            if (ex is ApiException)
            {
                exExd = (ex as ApiException).ErrorCode;
                content = new ApiExceptionProcess<RequestModelType, ResponseModelType>().Process(getErrorContent, ex, request, apiLogEntity);
            }
            else
            {
                content = new NocatchExceptionProcess<RequestModelType, ResponseModelType>().Process(getErrorContent, ex, request, apiLogEntity);
            }

            return content;
        }
    }
}
