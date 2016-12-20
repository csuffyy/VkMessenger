using Newtonsoft.Json;
using VkData.Interface;

namespace VkData.Helpers
{
    public class JsonSerializer : ISerializer
    {
        public string SerializeObject(object obj) =>
            JsonConvert.SerializeObject(obj, Formatting.Indented);

        public T DeserializeObject<T>(string s) => JsonConvert.DeserializeObject<T>(s);
    }
}