namespace VkData.Interface
{
    public interface IJsonParseStrategy<in TAccount, in TToken>
    {
        object Parse(TToken token, TAccount Account);
    }
}