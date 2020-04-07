using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace EFWService.OpenAPI.OutputProcessor
{
    public class XmlOutputProcessor : IOutputProcessor
    {
        public string OutPut<RequestModelType>(RequestModelType request)
        {
            XmlSerializer ser = new XmlSerializer(typeof(RequestModelType));

            using (MemoryStream stream = new MemoryStream(100))
            {
                ser.Serialize(stream, request);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
