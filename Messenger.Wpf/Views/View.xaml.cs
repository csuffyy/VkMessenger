using GalaSoft.MvvmLight.Ioc;
using MvvmService;
using WPFGrowlNotification;

namespace Messenger.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class View
    {
        public View()
        {
            InitializeComponent();
            SimpleIoc.Default.Register<IWPFAppSettings, WpfAppSettings>();
            SimpleIoc.Default.Register<IWPFAppResources, WpfAppResources>();
            SimpleIoc.Default.Register(WindowManager.GetNotifications);
        }
    }
}