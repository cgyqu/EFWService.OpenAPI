using EFWService.OpenAPI.Model;

namespace TestAPI.Core.Method
{
    public class TestModel : ApiRequestModelBase, IStructuredPost
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }


    public class TestResponseModel : ApiResponseModelBase
    {
        public TestModel request { get; set; }
    }
}
