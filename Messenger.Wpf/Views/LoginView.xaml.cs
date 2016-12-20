﻿using System.Windows;
using System.Windows.Controls;

namespace VkMessageWpf.Views
{
    /// <summary>
    ///     Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Login_OnClick(object sender, RoutedEventArgs e) =>
            Dispatcher.Invoke(() =>
                ((LoginViewModel) DataContext).ConfirmCredentials(PasswordBox.Password, LoginBox.Text));
    }
}