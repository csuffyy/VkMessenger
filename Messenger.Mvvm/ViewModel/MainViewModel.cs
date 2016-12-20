using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using VkData;
using VkData.Account.Categories;
using VkData.Account.Types;
using VkNet.Enums.Filters;
using Application = System.Windows.Application;
using Settings = VkMessageWpf.Properties.Settings;
using Timer = System.Timers.Timer;

namespace MvvmService.ViewModel
{
    /// <para>
    ///     See http://www.galasoft.ch/mvvm
    /// </para>
    public class MainViewModel : ViewModel<VkAccount, string, ViewModelBase>
    {
        private bool _authorised;
        private string _dialogText;
        private bool _isOpenDialog;

        public Action ShowProgress;
        private string _lastLoggerMessage;

        public MainViewModel()
        {
            ShowProgress = () =>
                ChildViewModel = new ProgressViewModel();

            Settings.Default.Upgrade();

            UserSettings = new UserSettings(
                () => Settings.Default.Password,
                () => Settings.Default.Login,
                () => Settings.Default.VkAppId,
                () => null,
                e => null,
                _ => Settings.Default.Password = _,
                _ => Settings.Default.Login = _,
                Settings.Default.Save
                );

            Callbacks = new VkCallbacks(
                () =>
                {
                    Authorised = false;
                    ChildViewModel = new ProgressViewModel();
                },
                Service =>
                {
                    Authorised = false;
                    ChildViewModel = new LoginViewModel(Service as Authentication);
                },
                () =>
                {
                    Authorised = true;
                    Notifications = new NotificationsViewModel(Account, SimpleIoc.Default);
                    ChildViewModel = new HomeViewModel(Account, LogOutCommand, ClearCache);
                },
                OnAuthorizationException,
                OnApiException);

            Account = DefaultAccount;
            Account.Authentication.Start();
        }

        private VkAccount DefaultAccount =>
            new VkAccount(VkData.Account.Types.AppSettings.Default(), UserSettings, Callbacks, SimpleIoc.Default);

        public VkCallbacks Callbacks { get; }
        public UserSettings UserSettings { get; }
        public NotificationsViewModel Notifications { get; set; }

        public bool Authorised
        {
            get { return _authorised; }
            set { Set(ref _authorised, value); }
        }

        public ICommand LogOutCommand => new RelayCommand(() => LogOut(true));

        public ICommand ClearCache => new RelayCommand(() =>
        {
            if (!Authorised)
                return;
            ChildViewModel = new ProgressViewModel();
            Task.Factory.StartNew(() =>
            {
                Account.Storage.ClearAll();
                ChildViewModel = new HomeViewModel(Account, LogOutCommand, ClearCache);
            }, CancellationToken.None,
               TaskCreationOptions.LongRunning,
               TaskScheduler.Default);

            Account.Logger.PropertyChanged += (s, e) => LastLoggerMessage = Account.Logger.LastLoggerMessage;
        });

        public bool IsOpenDialog
        {
            get { return _isOpenDialog; }
            set { Set(ref _isOpenDialog, value); }
        }

        public string DialogText
        {
            get { return _dialogText; }
            set { Set(ref _dialogText, value); }
        }

        public string LastLoggerMessage
        {
            get { return _lastLoggerMessage; }
            set { Set(ref _lastLoggerMessage ,value); }
        }

        private void LogOut(bool isWriteCacheRequested)
        {
            Settings.Default.Reset();
            DialogText = "Logged out";
            IsOpenDialog = true;

            if (isWriteCacheRequested)
                Account.Storage.WriteAll();

            Account.Notifications.Cancel();
            Account = new VkAccount(VkData.Account.Types.AppSettings.Default(), UserSettings, Callbacks, SimpleIoc.Default);
            Account.Authentication.Start();
        }

        #region Helper stuff

        private void OnApiException()
        {
            DialogText = "Please check internet connection";
            IsOpenDialog = true;
            var t = new Timer(2500);
            t.Start();
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }

        private void OnAuthorizationException(Exception e)
        {
            DialogText = "Please check internet connection and re-authorize";
            IsOpenDialog = true;
            LogOut(false);
        }

        #endregion
    }
}