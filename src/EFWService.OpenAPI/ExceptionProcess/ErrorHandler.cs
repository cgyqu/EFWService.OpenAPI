using System;
using System.Web;
using EFWService.OpenAPI.DynamicController;
using EFWService.OpenAPI.Exceptions;
using EFWService.OpenAPI.Logger;
using EFWService.OpenAPI.Model;

namespace EFWService.OpenAPI.ExceptionProcess
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
            string content = string.Empty;
            if (ex is ApiException)
            {
                content = new ApiExceptionProcess<RequestModelType, ResponseModelType>().Process(getErrorContent, ex, request, apiLogEntity);
            }
            else if (ex is HttpException)
            {
                content = new HttpExceptionProcess<RequestModelType, ResponseModelType>().Process(getErrorContent, ex, request, apiLogEntity);
            }
            else
            {
                content = new NocatchExceptionProcess<RequestModelType, ResponseModelType>().Process(getErrorContent, ex, request, apiLogEntity);
            }

            return content;
        }
    }
}
