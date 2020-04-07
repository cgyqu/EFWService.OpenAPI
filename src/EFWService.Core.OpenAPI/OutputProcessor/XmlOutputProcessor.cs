using System.IO;
using System.Text;
using System.Xml.Serialization;
using EFWService.Core.OpenAPI.OutputProcessor;
using EFWService.Core.OpenAPI.Exs;

namespace EFWService.Core.OpenAPI.OutputProcessor
{
    public class XmlOutputProcessor : IOutputProcessor
    {
        public string OutPut<ResponseModel>(ResponseModel model)
        {
            return XmlSerializeEx.CustomXmlSerialize(model, removeXsd: true, removeXsi: true);
        }
    }
}
