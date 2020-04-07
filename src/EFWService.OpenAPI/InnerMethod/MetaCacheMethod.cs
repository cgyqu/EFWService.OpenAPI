using EFWService.OpenAPI.Attributes;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Model;
using EFWService.OpenAPI.Utils;
using System.Linq;

namespace EFWService.OpenAPI
{
    [ApiMethodDesc(module: "API",
        category: "Method",
        httpMethodType: HttpMethodType.GET,
        desc: "接口信息", isShow: false)]
    public class MetaCacheMethod : ApiMethodBase<ApiRequestModelBase, NormalResponseModel>
    {
        public override NormalResponseModel ExecuteLogic(ApiRequestModelBase request)
        {
            return new NormalResponseModel() { };
        }

        public override string CustomOutputFun(ApiRequestModelBase request, NormalResponseModel response)
        {
            return JsonConvertExd.SerializeObject(WebBaseUtil.ApiMethodMetaCache.Where(x => x.Value.APIMethodDesc.IsShow).Select(c => c.Value));
        }
    }
}