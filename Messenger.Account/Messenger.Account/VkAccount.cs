using System;
using System.Collections.Specialized;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using VkData.Account.Categories;
using VkData.Account.Enums;
using VkData.Account.Interface;
using VkData.Account.Types;
using VkData.Helpers;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using History = VkData.Account.Categories.History;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace VkData
{
    public class VkAccount : IVkAccount
    {
        public VkAccount(IAppSettings appSettings, IUserSettings userSettings, VkCallbacks callbacks,
            IServiceLocator locator)
        {
            AppSettings = appSettings;
            Callbacks = callbacks;
            UserSettings = userSettings;

            VkApi = new VkApi();

            Logger = ((Func<ILogger>)(() =>
           {
               var logger = locator.GetInstance<ILogger>();
               logger.Path = AppSettings.LogPath;
               return logger;
           })).
                TryOr(e => new FileLogger(AppSettings.LogPath)).Value;

            var serialiser = ((Func<ISerializer>)(locator.GetInstance<ISerializer>)).
                TryOr(e => new JsonSerializer()).Value;

            Storage = new Storage(appSettings.StoragePath,
                serialiser,
                Logger,
                Refresh);
            Downloader = new Downloader(appSettings.BaseDirectory);

            Chats = new Chats(this);
            Users = new Users(this);
            Stickers = new Stickers(this);
            Avatars = new Avatars(this);
            Photos = new Photos(this);
            History = new History(this);
            Notifications = new VkNotifications(this);
            Authorization = new Authorization(this);
            CancellationTokenSource = new CancellationTokenSource();
            Authentication = new Authentication(this, UserSettings, true);
        }


        public void ResetSettings(string userFolder)
        {
            AppSettings.UserFolder = userFolder;

            Logger.Path = AppSettings.LogPath;
            Storage.Path = AppSettings.StoragePath;
        }


        public void Refresh()
        {
            var current = Users.Current;
            ResetSettings(current);

            Storage.LoadAll();
            Users.Update();
            Chats.Update();
            History.GetUnreadHistory();
            Notifications.Start();
            Avatars.DownloadAvatars();
        }

        #region IAccount support

        public bool IsAuthorized { get; set; }

        public void RaiseAuthenticated<T>(IUserSettings settings, Action<T> onFailure, T parameter)
        {
            if (settings.Empty)
            {
                onFailure?.Invoke(parameter);
                return;
            }
            Authenticated?.Invoke(this, new AuthenticatedEventHandlerArgs(settings));
        }

        public event Action Authorized;

        public event NotifyCollectionChangedEventHandler ReceivedNotification
        {
            add { Notifications.Notifications.CollectionChanged += value; }
            remove { Notifications.Notifications.CollectionChanged -= value; }
        }

        
        public void RaiseUserAuthorized()
        {
            Authorized?.Invoke();
        }

        public AccountCallbacks<Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, PhotoSize> Callbacks { get; }
        public VkApi VkApi { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public LongPollServerResponse LongPollServer { get; set; }
        public IAppSettings AppSettings { get; }
        public IUserSettings UserSettings { get; }
        public Downloader Downloader { get; }
        public ILogger Logger { get; }

        public IAvatars<Message> Avatars { get; }
        public IPhotos<Photo, PhotoSize> Photos { get; }
        public IStickers<Message, StickerSize> Stickers { get; }
        public IStorage<Message, LongPollServerSettings, Chat, User> Storage { get; }

        public
            IHistory
               <Message, LongPollServerSettings>
            History
        { get; }

        public
            IAuthenticationService
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, PhotoSize>
            Authentication
        { get; }

        public IChats<Chat> Chats { get; }
        public IUsers<Message, User> Users { get; }
        public INotifications<Message, LongPollServerResponse> Notifications { get; }
        public event AuthenticatedEventHandler Authenticated;
        public IAuthorization Authorization { get; }

        #endregion
    }
}