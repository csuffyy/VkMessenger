using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using VkData.Account.Enums;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace MvvmService.ViewModel
{
    public static class MessageExtensions
    {
        public static ViewModelBase ToViewModel(this Message message,
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, PhotoSize, StickerSize>
                account)
        {
            return new MessageViewModel(account, message);
        }

        public static List<MessageViewModel> ToViewModels(this IEnumerable<Message> messages,
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, PhotoSize, StickerSize>
                account)
        {
            return messages.Select(m => new MessageViewModel(account, m)).ToList();
        }
    }
}