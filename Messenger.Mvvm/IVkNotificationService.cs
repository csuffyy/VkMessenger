using VkData.Account.Enums;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace MvvmService
{
    public interface IVkNotificationService : INotificationService
        <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
            StickerSize, PhotoSize>
    {
    }
}