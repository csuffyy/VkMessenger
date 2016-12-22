using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MvvmService.ViewModel;
using VkData.Interface;

namespace WPFGrowlNotification
{
    public partial class GrowlNotifications : INotificationProvider<MessageViewModel>
    {
        private const double TopOffset = 20;
        private const double LeftOffset = 380;
        private const byte MAX_NOTIFICATIONS = 4;

        private readonly ObservableCollection<MessageViewModel> buffer =
            new ObservableCollection<MessageViewModel>();

        private int count;

        public ObservableCollection<MessageViewModel> Notifications =
            new ObservableCollection<MessageViewModel>();

        public GrowlNotifications(double topOffset, double leftOffset)
        {
            InitializeComponent();
            Top = GetDefaultTop(topOffset);
            Left = GetDefaultLeft(leftOffset);
            NotificationsControl.DataContext = Notifications;
        }

        public GrowlNotifications() : this(TopOffset, LeftOffset)
        {
        }

        public Action<MessageViewModel> Add => AddNotification;

        private static double GetDefaultLeft(double leftOffset)
            => SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - leftOffset;

        private static double GetDefaultTop(double topOffset) => SystemParameters.WorkArea.Top + topOffset;

        public void AddNotification(MessageViewModel MessageViewModel)
        {
            MessageViewModel.Id = count++;
            if (Notifications.Count + 1 > MAX_NOTIFICATIONS)
                buffer.Add(MessageViewModel);
            else
                Notifications.Add(MessageViewModel);

            //Show window if there're notifications
            if (Notifications.Count > 0 && !IsActive)
                Show();
        }

        public void RemoveNotification(MessageViewModel MessageViewModel)
        {
            if (Notifications.Contains(MessageViewModel))
                Notifications.Remove(MessageViewModel);

            if (buffer.Count > 0)
            {
                Notifications.Add(buffer[0]);
                buffer.RemoveAt(0);
            }

            //Close window if there's nothing to show
            if (Notifications.Count < 1)
                Hide();
        }

        private void NotificationWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Math.Abs(e.NewSize.Height) > 0.0)
                return;
            var element = sender as Grid;
            RemoveNotification(Notifications.First(n => element != null && n.Id == int.Parse(element.Tag.ToString())));
        }
    }
}