using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Messenger.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }
        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            HamburgerMenuControl.Content = e.ClickedItem;
        }

        private void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuItem;
        }
    }
}