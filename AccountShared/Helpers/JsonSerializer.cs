using Newtonsoft.Json;
using VkData.Interface;

namespace VkData.Helpers
{
    public class JsonSerializer : ISerializer
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };

        public string SerializeObject(object obj) =>
            JsonConvert.SerializeObject(obj, settings);

        public T DeserializeObject<T>(string s) => JsonConvert.DeserializeObject<T>(s, settings);
    }
}