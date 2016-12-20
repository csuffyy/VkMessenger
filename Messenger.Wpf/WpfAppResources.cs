using Messenger.Wpf.Properties;
using MvvmService;

namespace Messenger.Wpf
{
    internal sealed class WpfAppResources : IWPFAppResources
    {
        public string TooShortPasswordMessage => Resources.TooShortPasswordMessage;

        public string IncorrectLoginMessage => Resources.IncorrectLoginMessage;
    }
}