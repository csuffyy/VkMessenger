using VkData.Account.Enums;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace VkData.Account.Interface
{
    public interface IVkAccount :
        IAccount<Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, Enums.PhotoSize, StickerSize>
    {
    }
}