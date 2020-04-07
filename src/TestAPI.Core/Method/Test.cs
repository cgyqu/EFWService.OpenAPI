using EFWService.Core.OpenAPI;
using EFWService.Core.OpenAPI.Attributes;
using EFWService.OpenAPI.Emums;
using TestAPI.Core.Method;

namespace TestAPI.Method
{
    [ApiMethodDesc(module: "Test",
        category: "Query",
        httpMethodType: HttpMethodType.POST,
        desc: "测试接口")]
    //path:api/test/Query/SimpleTest
    public class SimpleTest : ApiMethodBase<TestModel, TestResponseModel>
    {
        public override TestResponseModel ExecuteLogic(TestModel request)
        {
            TestResponseModel response = new TestResponseModel();
            response.HasResult();
            response.request = request;
            return response;
        }
    }
}