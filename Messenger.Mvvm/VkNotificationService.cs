using System;
using System.Collections.Specialized;
using System.Linq;
using MvvmService.ViewModel;
using VkData;
using VkData.Account.Enums;
using VkData.Account.Extension;
using VkData.Account.Interface;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace MvvmService
{
    public class VkNotificationService :
       IVkNotificationService
    {
        private readonly Action<MessageViewModel> _doAdd;

        public VkNotificationService(IVkAccount account, bool executeLast)
        {
            Account = account ;
            ExecuteLast = executeLast;
        }

        public VkNotificationService(IVkAccount account, Action<MessageViewModel> doAdd, bool executeLast)
            : this(account, executeLast)
        {
            _doAdd = doAdd;
        }


        public VkNotificationService(IVkAccount account, INotificationProvider<MessageViewModel> provider,
            bool executeLast) : this(account, executeLast)
        {
            _doAdd = provider.Add;
        }

        public bool ExecuteLast { get; set; }

        public
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, PhotoSize, StickerSize>
            Account { get; }

        public void StartTracking()
        {
            Account.ReceivedNotification += OnReceived;
        }

        public void StopTracking()
        {
            Account.ReceivedNotification -= OnReceived;
        }

        private void OnReceived(object sender, NotifyCollectionChangedEventArgs notificationArgs)
        {
            var items = (ObservableConcurrentDictionary<string, Dialog<Message>>)sender;
            var _items = items.SelectMany(item => item.Value.Projection);

            foreach (var message in _items)
            {
                if (Account.Users.GetFullUserName(message) == Account.Users.Current)
                    return;

                var attachments = message.GetAttachments().ToList();
                var list = 
                      Account.Photos.GetPathByAttachments(attachments, PhotoSize.Photo130);
                var sticker = Account.Stickers.Get(attachments, StickerSize.Photo64);

                var notification = new MessageViewModel(Account, message);

                if (message.Body.Length != 0 || sticker != null)
                    AddNotification(notification);

                foreach (var item in list)
                {
                    notification = new MessageViewModel(Account, message);
                    AddNotification(notification);
                }
            }
        }

        private void AddNotification(MessageViewModel MessageViewModel)
        {
            _doAdd(MessageViewModel);
        }
    }
}