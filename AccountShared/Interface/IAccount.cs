using System;
using System.Collections.Specialized;
using System.Threading;
using VkData.Helpers;

#pragma warning disable 618

namespace VkData.Interface
{
    public interface IAccount<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto,
        TPhotoSize, TStickerSize>
    {
        AccountCallbacks
            <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize, TPhotoSize
                > Callbacks { get; }

        TApi VkApi { get; }
        CancellationTokenSource CancellationTokenSource { get; }
        TResponse LongPollServer { get; set; }
        IAppSettings AppSettings { get; }
        IUserSettings UserSettings { get; }
        Downloader Downloader { get; }
        ILogger Logger { get; }
        IAvatars<TMessage> Avatars { get; }
        IPhotos<TPhoto, TPhotoSize> Photos { get; }
        IStickers<TMessage, TStickerSize> Stickers { get; }
        IStorage<TMessage, TPollSettings, TChat, TUser> Storage { get; }
        IHistory<TMessage, TPollSettings> History { get; }

        IAuthenticationService
            <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize, TPhotoSize
                > Authentication { get; }

        IChats<TChat> Chats { get; }
        IUsers<TMessage, TUser> Users { get; }
        IAuthorization Authorization { get; }
        INotifications<TMessage, TResponse> Notifications { get; }
        bool IsAuthorized { get; set; }
        event AuthenticatedEventHandler Authenticated;
        void RaiseAuthenticated<T>(IUserSettings settings, Action<T> onFailure, T parameter);
        event Action Authorized;
        event NotifyCollectionChangedEventHandler ReceivedNotification;
        void RaiseUserAuthorized();
        void ResetSettings(string userFolder);
        void Refresh();
    }
}