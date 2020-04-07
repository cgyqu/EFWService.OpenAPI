using EFWService.Core.OpenAPI.OutputProcessor;
using EFWService.Core.OpenAPI.Utils;

namespace EFWService.Core.OpenAPI.OutputProcessor
{
    public class JsonOutputProcessor : IOutputProcessor
    {
        public string OutPut<ResponseModel>(ResponseModel model)
        {
            return JsonConvertExd.SerializeObject(model);
        }
    }
}
