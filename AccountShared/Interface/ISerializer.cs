namespace VkData.Interface
{
    public interface ISerializer
    {
        string SerializeObject(object obj);
        T DeserializeObject<T>(string s);
    }
}