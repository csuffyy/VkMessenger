using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using VkData;
using VkData.Account.Interface;
using VkData.Helpers;
using Application = System.Windows.Application;
using History = VkData.Account.Categories.History;

namespace MvvmService.ViewModel
{
    public class DialogViewModel : ViewModelBase
    {
        private string _avatar;
        private string _body;
        private string _dialogName;
        private bool _isMessageInComing;

        private ObservableCollection<MessageViewModel> _messages;
        private int _messagesGetOffset;
        /// <summary>
        /// Creates a new instance of DialogViewModel.
        /// Multiline comment below should be removed in release.
        /// Currently it's used for debugging.
        /// </summary>
        /// <param name="account">IVkAccount instance</param>
        /// <param name="dialogName">Dialog name</param>
        /// <param name="showProgess">Action which shows a progress to user</param>
        public DialogViewModel(IVkAccount account, string dialogName, Action showProgess)
        {
            Account = account;
            DialogName = dialogName;
            ShowProgess = showProgess;
    
            NotificationService = new VkNotificationService(account, vm =>
            {
                /*if (vm.ImageUrl == Account.Avatars.FriendsDictionary[Account.Users.Current])
                    return;
                    */
                IsMessageInComing = true;
                Messages.Add(vm);
            }, false);
            NotificationService.StartTracking();
        }

        public VkNotificationService NotificationService { get; }

        public ObservableCollection<MessageViewModel> Messages
        {
            get { return _messages; }
            set { Set(ref _messages, value); }
        }

        public IVkAccount Account { get; set; }

        public bool IsMessageInComing
        {
            get { return _isMessageInComing; }
            set { Set(ref _isMessageInComing, value); }
        }

        public string DialogName
        {
            get { return _dialogName; }
            set { Set(ref _dialogName, value); }
        }

        public Action ShowProgess { get; set; }

        public string Avatar
        {
            get { return _avatar; }
            set { Set(ref _avatar, value); }
        }

        public string Body
        {
            get { return _body; }
            set { Set(ref _body, value); }
        }

        public ICommand LoadPrevious => new RelayCommand(() =>
        {
            _messagesGetOffset += History.VkMessagesOffset;
            ShowProgess();
            UpdateAsync(_messagesGetOffset);
        });

        public ICommand LoadNext => new RelayCommand(() =>
        {
            if (_messagesGetOffset == 0) return;
            _messagesGetOffset -= History.VkMessagesOffset;
            ShowProgess();
            UpdateAsync(_messagesGetOffset);
        });

        public ICommand SendMessage => new RelayCommand(() =>
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(Body) || string.IsNullOrWhiteSpace(Body))
                    return;
                var body = Body;
                Body = string.Empty;
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    IsMessageInComing = true;
                    if (_messagesGetOffset != 0)
                        Update(0);
                    Messages.Add(new MessageViewModel
                    {
                        Message = body.GetText(Account.Users.Current),
                        ImageUrl = await Account.Avatars.Get(Account.Users.Current),
                        Timestamp = DateTime.Now.ToShortTimeString()
                    });
                });

                Account.History.SendMessage(body, DialogName);
            });
        });

        public ICommand ShowPhotoInViewer => new RelayCommand<string>(s => Process.Start((string) s));

        public Action OnEnd { get; set; }

        public async void UpdateAsync(long offset)
        {
            Avatar = await Account.Avatars.Get(DialogName);
            await
                Task.Factory.StartNew(
                    () => Update(offset),
                    CancellationToken.None, 
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
        }

        private void Update(long offset)
        {
            var dialog =
                Account.History.GetHistory(DialogName, offset, History.MaxMessagesCount).Value;

            if (dialog.Messages.Count == 0)
                dialog =
                    Account.History.GetHistory(DialogName, offset, null).Value;

            var observableCollection = dialog.Messages[offset].ToViewModels(Account).ToObservable();
            Messages = observableCollection;
            OnEnd?.Invoke();
            IsMessageInComing = true;
        }
    }
}