﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using VkData;
using VkNet.Enums.Filters;

namespace MvvmService.ViewModel
{
    public class HomeViewModel : ViewModel<VkAccount, Type, ViewModelBase>
    {
        public HomeViewModel(VkAccount account, ICommand logOut, ICommand clearCache) : base(account)
        {
            LogOut = logOut;
            ClearCache = clearCache;
            SetViewModelToFriendsCommand.Execute(null);
            VkViewModel?.SelectLastDialog();
        }

        private VkViewModel VkViewModel => ChildViewModel as VkViewModel;

        public ICommand CollapseCommand
        {
            get
            {
                return new RelayCommand(() => VkViewModel.ContacsCollapsed = !VkViewModel.ContacsCollapsed);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var model = VkViewModel;
                    if (model != null)
                        model.CanSearch = !model.CanSearch;
                });
            }
        }

        public ICommand SetViewModelToFriendsCommand
            => new RelayCommand(() => SetViewModel<FriendsViewModel>(a => new FriendsViewModel(a)));

        public ICommand SetViewModelToChatsCommand
            => new RelayCommand(() => SetViewModel<ChatsViewModel>(a => new ChatsViewModel(a)));

        public ICommand QuitCommand => new RelayCommand(() =>
        {
            Settings.Default.LastDialog = VkViewModel?.LastDialog;
            Settings.Default.Save();
            Account.Storage.WriteAll();
            Application.Current.Dispatcher.Invoke(
                Application.Current.Shutdown,
                DispatcherPriority.Send);
        });

        public ICommand ClearCache { get; }
        public ICommand LogOut { get; }
        private void SetViewModel<T>(Func<VkAccount, ViewModelBase> @new) where T : ViewModelBase
        {
            if (ChildViewModel is T)
                return;

            Task.Factory.StartNew(() =>
            {
                if (_cachedViewModels.ContainsKey(typeof (T)))
                {
                    ChildViewModel = _cachedViewModels[typeof (T)];
                    return;
                }
                var vm = @new(Account);
                _cachedViewModels[typeof (T)] = vm;
                ChildViewModel = vm;
            }, CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            ChildViewModel = ProgressViewModel;
        }
    }
}