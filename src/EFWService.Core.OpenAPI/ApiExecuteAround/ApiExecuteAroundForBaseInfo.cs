using EFWService.Core.OpenAPI.Utils;

namespace EFWService.Core.OpenAPI
{
    public class ApiExecuteAroundForBaseInfo : DefaultApiExecuteAround
    {
        public override void Before(BeforeParam param)
        {
        }

        public override void After(AfterParam param)
        {
            param.Stopwatch.Stop();
            param.HttpResponse.Headers.Add("es", param.Stopwatch.ElapsedMilliseconds.ToString());
            param.HttpResponse.Headers.Add("fap", param.ApiMethodMetaInfo.Fap);
        }
    }
}
