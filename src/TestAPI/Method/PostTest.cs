using System.Web.Mvc;
using EFWService.OpenAPI;
using EFWService.OpenAPI.Attributes;
using EFWService.OpenAPI.Emums;
using EFWService.OpenAPI.Model;
using EFWService.OpenAPI.Models;
using EFWService.OpenAPI.Utils;

namespace TestAPI.Method
{
    [ApiMethodDesc(module: "MyTest",
        category: "Query",
        httpMethodType: HttpMethodType.POST,
        desc: "测试接口")]
    //path :mytest/quest/test
    public class PostTestMethod : ApiMethodBase<PostRequest, NormalResponseModel>
    {
        public override RequestParamsCheckResult RequestParamsCheck(ModelStateDictionary modelState, PostRequest request, ParamsCheckHelper paramsCheckHelper)
        {
            return paramsCheckHelper.Check(()=>request.Id > 0,"Id必须大于0").Finish();
        }
        public override NormalResponseModel ExecuteLogic(PostRequest request)
        {
            NormalResponseModel response = new NormalResponseModel();
            response.HasResult();
            response.result = request;
            return response;
        }
    }

    public class PostRequest : ApiRequestModelBase, IStructuredPost
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}