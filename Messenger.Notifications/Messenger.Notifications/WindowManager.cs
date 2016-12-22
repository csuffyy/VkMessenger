using MvvmService.ViewModel;
using VkData.Interface;

namespace WPFGrowlNotification
{
    public static class WindowManager
    {
        public static INotificationProvider<MessageViewModel> GetNotifications()
        {
            return new GrowlNotifications();
        }
    }
}