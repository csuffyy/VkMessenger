using VkData.Account.Enums;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace VkData.Account.Interface
{
    public interface IVkAuthService :
        IAuthenticationService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, Enums.PhotoSize>
    {
    }
}