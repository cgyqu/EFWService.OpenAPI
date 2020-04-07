using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Exceptions;

namespace EFWService.OpenAPI.Authentication
{
    /// <summary>
    /// HTTP请求方式验证器
    /// </summary>
    public class HttpMethodAuthentication : Authentication
    {
        public override void Verify<RequestModelType, ResponseModelType>(ApiMethodBase<RequestModelType, ResponseModelType> apiMethodBase, RequestModelType request)
        {
            string configMethod = apiMethodBase.ApiMethodMetaInfo.APIMethodDesc.HttpMethodType.ToString().ToUpper();
            string currentMethod = apiMethodBase.HttpMethod.ToUpper();
            bool flag = false;
            if (configMethod == HttpMethodType.ALL.ToString())
            {
                flag = true;
            }
            else
            {
                flag = currentMethod == apiMethodBase.ApiMethodMetaInfo.APIMethodDesc.HttpMethodType.ToString().ToUpper();
            }
            if (flag == false)
            {
                throw new ApiException(ApiResultCode.HttpMethodError) { ErrorMessage = string.Format("该接口必须以:{0}方式请求", configMethod) };
            }
        }
    }
}
