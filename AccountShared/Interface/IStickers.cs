using System.Collections.Generic;
using System.Threading.Tasks;

namespace VkData.Interface
{
    public interface IStickers<in TMessage, in TStickerSize>
    {
        Task<string> Get(IEnumerable<object> attachments, TStickerSize size);
        Task<string> Get(TMessage message, TStickerSize size);
        Task<string> Get(string url);
        string GetSize(string url, string extension);
        string GetId(string url);
    }
}