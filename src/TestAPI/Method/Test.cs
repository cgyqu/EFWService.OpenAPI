using EFWService.OpenAPI;
using EFWService.OpenAPI.Attributes;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Model;

namespace TestAPI.Method
{
    [ApiMethodDesc(module: "MyTest",
        category: "Query",
        httpMethodType: HttpMethodType.GET,
        desc: "测试接口")]
    //path :mytest/quest/test
    public class TestMethod : ApiMethodBase<ApiRequestModelBase, NormalResponseModel>
    {
        public override NormalResponseModel ExecuteLogic(ApiRequestModelBase request)
        {
            NormalResponseModel response = new NormalResponseModel();
            response.HasResult();
            response.result = "test";
            return response;
        }
    }
}