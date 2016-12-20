using System.Media;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using VkData.Account.Categories;
using VkData.Account.Interface;
using VkData.Interface;

namespace MvvmService.ViewModel
{
    public class NotificationsViewModel : ViewModelBase
    {
        private readonly SoundPlayer Player;

        public NotificationsViewModel(IVkAccount account, IServiceLocator ioc)
        {
            Player = new SoundPlayer(account.AppSettings.NotificationSoundPath);
            Player.LoadAsync();

            var doAdd =
                Application.Current.Dispatcher.Invoke(ioc.GetInstance<INotificationProvider<MessageViewModel>>).Add;

            Service =
                new VkNotificationService(account, vm =>
                {
                    doAdd(vm);
                    Player.Play();
                }, true);
            Service.StartTracking();
        }

        public NotificationsViewModel(Authentication service, IServiceLocator ioc)
            : this(service.Account as IVkAccount, ioc)
        {
        }

        protected VkNotificationService Service { get; set; }
    }
}