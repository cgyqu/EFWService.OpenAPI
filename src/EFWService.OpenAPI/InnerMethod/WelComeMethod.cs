using System.Linq;
using EFWService.OpenAPI.Attributes;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Model;
using EFWService.OpenAPI.Utils;

namespace EFWService.OpenAPI
{
    [ApiMethodDesc(module: "Home",
        category: "WelCome",
        httpMethodType: HttpMethodType.GET,
        desc: "欢迎首页", isShow: false)]
    public class IndexMethod : ApiMethodBase<ApiRequestModelBase, NormalResponseModel>
    {
        public override NormalResponseModel ExecuteLogic(ApiRequestModelBase request)
        {
            return new NormalResponseModel() { };
        }

        public override string CustomOutputFun(ApiRequestModelBase request, NormalResponseModel response)
        {
            var message = new { message = "WelCome to OpenAPI!" };
            return JsonConvertExd.SerializeObject(message);
        }
    }
}