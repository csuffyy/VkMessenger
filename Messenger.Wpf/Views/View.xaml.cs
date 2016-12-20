using System.ComponentModel;

namespace VkMessageWpf.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class View
    {
        public View()
        {
            InitializeComponent();
            /*
            NIcon.DoubleClick +=
                (sender, args) =>
                {
                    Show();
                    WindowState = WindowState.Normal;
                };*/
        }
        /*
        private NotifyIcon NIcon { get; } = new NotifyIcon
        {
            Icon = new Icon(@"D:\projects\C#\VkMessengerWpf\VkMessageWpf\vk.ico"),
            Visible = false
        };
        */

        protected override void OnClosing(CancelEventArgs e)
        {
           (((MainViewModel)DataContext).ChildViewModel as HomeViewModel)?
                .QuitCommand.Execute(null);
        }

        /*
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }*/
    }
}