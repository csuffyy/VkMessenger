using GalaSoft.MvvmLight;

namespace Messenger.Wpf.Views
{
    public class DemoItem : ViewModelBase
    {
        private string _name;
        private object _content;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public object Content
        {
            get { return _content; }
            set { Set(ref _content, value); }
        }
    }
}
