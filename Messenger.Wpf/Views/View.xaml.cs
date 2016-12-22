using System.ComponentModel;
using GalaSoft.MvvmLight.Ioc;
using MvvmService;
using MvvmService.ViewModel;
using WPFGrowlNotification;

namespace Messenger.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class View
    {
        static View()
        {
            SimpleIoc.Default.Register<IWPFAppSettings, WpfAppSettings>();
            SimpleIoc.Default.Register<IWPFAppResources, WpfAppResources>();
            SimpleIoc.Default.Register(WindowManager.GetNotifications);
        }

        public View()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ((MainViewModel) DataContext).QuitCommand.Execute(null);
        }
    }
}