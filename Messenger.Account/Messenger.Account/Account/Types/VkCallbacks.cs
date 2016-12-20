using System;
using VkData.Account.Enums;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace VkData.Account.Types
{
    public class VkCallbacks : AccountCallbacks<Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, Enums.PhotoSize>
    {
        public VkCallbacks(Action onAuthenticating, Action<IAuthenticationService<Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, PhotoSize>> onAuthenticationFailure, Action onAuthorized, Action<Exception> onAuthorizationException, Action onApiException) : base(onAuthenticating, onAuthenticationFailure, onAuthorized, onAuthorizationException, onApiException)
        {
        }

        public VkCallbacks(AccountCallbacks<Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, PhotoSize> callbacks) : base(callbacks)
        {
        }
    }
}