using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EFWService.OpenAPI.Utils
{
    public static class JsonConvertExd
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObject(this object value, string datetimeFormart = "yyyy-MM-dd HH:mm:ss.fff")
        {
            IsoDateTimeConverter dateC = new IsoDateTimeConverter() { DateTimeFormat = datetimeFormart };
            var formatting = Formatting.None;
            var converters = new JsonConverter[] { dateC };
            JsonSerializerSettings settings = new JsonSerializerSettings { Converters = converters };
            string json = JsonConvert.SerializeObject(value, formatting, settings);
            return json;
        }

        public static T Deserialize<T>(string value, string datetimeFormart = "yyyy-MM-dd HH:mm:ss.fff")
        {
            IsoDateTimeConverter dateC = new IsoDateTimeConverter() { DateTimeFormat = datetimeFormart };
            var converters = new JsonConverter[] { dateC };
            JsonSerializerSettings settings = new JsonSerializerSettings { Converters = converters };
            return JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}
