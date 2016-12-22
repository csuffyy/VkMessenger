using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using VkData.Account.Categories;
using VkData.Account.Enums;
using VkData.Account.Extension;
using VkData.Account.Interface;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using Application = System.Windows.Application;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace MvvmService.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        private bool _hasEmoji;
        private int _id;
        private string _imageUrl;
        private string _message;
        private ObservableCollection<string> _photos;
        private string _stickerUri;
        private string _timestamp;
        private bool _fromMe;

        public MessageViewModel()
        {
        }

        public MessageViewModel(
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams,
                    Photo, PhotoSize, StickerSize>
                account, Message message)
        {
            Initialize((IVkAccount) account, message);
        }

        public string Timestamp
        {
            get { return _timestamp; }
            set { Set(ref _timestamp, value); }
        }

        public ObservableCollection<string> Photos
        {
            get { return _photos; }
            set { Set(ref _photos, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public bool FromMe
        {
            get { return _fromMe; }
            set { Set(ref _fromMe , value); }
        }

        public bool HasEmoji
        {
            get { return _hasEmoji; }
            set { Set(ref _hasEmoji, value); }
        }

        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { Set(ref _imageUrl, value); }
        }

        public string Sticker
        {
            get { return _stickerUri; }
            set { Set(ref _stickerUri, value); }
        }

        private async void Initialize(IVkAccount account, Message message)
        {
            var attachments = message.GetAttachments().ToList();
            var list = await account.Photos.GetPathByAttachments(attachments, PhotoSize.Photo604);

            Application.Current.Dispatcher.Invoke(() => Photos = list.ToObservable());

            Message = account.History.GetText(message);
            Timestamp = message.GetTime();
            SetImage(account, message);
            if (attachments.Count != 0 && attachments[0] is Sticker)
                Sticker = await account.Stickers.Get(attachments, StickerSize.Photo256);
            HasEmoji = message.ContainsEmojiSmiles;
            FromMe = account.Users.FromCurrent(message);
        }

        private async void SetImage(
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams,
                    Photo, PhotoSize, StickerSize>
                account, Message message)
        {
            ImageUrl = await account.Avatars.Get(message);
        }
    }
}