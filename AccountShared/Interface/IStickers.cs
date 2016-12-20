using System.Collections.Generic;

namespace VkData.Interface
{
    public interface IStickers<in TMessage, in TStickerSize>
    {
        string Get(IEnumerable<object> attachments, TStickerSize size);
        string Get(TMessage message, TStickerSize size);
        string Get(string url);
        string GetSize(string url, string extension);
        string GetId(string url);
    }
}