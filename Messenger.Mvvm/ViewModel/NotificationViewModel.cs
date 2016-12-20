using VkData;
using VkNet.Model;

namespace MvvmService.ViewModel
{
    public class NotificationViewModel : MessageViewModel
    {
        private string _photo;
        private string title;

        public NotificationViewModel(VkAccount account, Message message) : base(account, message)
        {
        }

        public static int DefaultHeight => 130;

        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        public string Photo
        {
            get { return _photo; }
            set { Set(ref _photo, value); }
        }

        public int Height => Photo == null ? DefaultHeight : 150;
    }
}