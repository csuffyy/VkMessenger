using Messenger.Wpf.Properties;
using MvvmService;

namespace Messenger.Wpf
{
    internal sealed class WpfAppSettings : IWPFAppSettings
    {
        public string Password
        {
            get { return Settings.Default.Password; }
            set { Settings.Default.Password = value; }
        }

        public string Login
        {
            get { return Settings.Default.Login; }
            set { Settings.Default.Login = value; }
        }

        public ulong VkAppId => Settings.Default.VkAppId;

        public string LastDialog
        {
            get { return Settings.Default.LastDialog; }
            set { Settings.Default.LastDialog = value; }
        }

        public void Upgrade()
        {
            Settings.Default.Upgrade();
        }

        public void Save()
        {
            Settings.Default.Save();
        }

        public void Reset()
        {
            Settings.Default.Reset();
        }

        public void Reload()
        {
            Settings.Default.Reload();
        }
    }
}