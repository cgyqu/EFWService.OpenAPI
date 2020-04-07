using EFWService.OpenAPI.Model;

namespace EFWService.OpenAPI.Authentication
{
    /// <summary>
    /// 验证接口
    /// </summary>
    public abstract class Authentication
    {
        /// <summary>
        /// 执行验证
        /// </summary>
        /// <param name="apiMethodBase"></param>
        public abstract void Verify<RequestModelType, ResponseModelType>(ApiMethodBase<RequestModelType, ResponseModelType> apiMethodBase, RequestModelType request)
            where RequestModelType : ApiRequestModelBase
            where ResponseModelType : ApiResponseModelBase;
    }
}
