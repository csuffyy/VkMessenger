using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using VkData.Account.Interface;
using VkNet.Enums.Filters;

namespace MvvmService.ViewModel
{
    public abstract class VkViewModel : ViewModel<IVkAccount, string, DialogViewModel>
    {
        private List<KeyValuePair<string, string>> _avatars;

        public List<KeyValuePair<string, string>> Avatars
        {
            get { return _avatars; }
            set { Set(ref _avatars, value); }
        }

        protected abstract List<KeyValuePair<string, string>> AvatarsImpl { get; }
        public ICommand SearchCommand { get; }

        public bool ContacsCollapsed
        {
            get { return _contacsCollapsed; }
            set { Set(ref _contacsCollapsed, value) ; }
        }

        private bool _canSearch;
        private bool _contacsCollapsed;

        public bool CanSearch
        {
            get { return _canSearch; }
            set { Set(ref _canSearch, value); }
        }
        public string LastDialog { get; internal set; }
        public ICommand SelectCurrent { get; }

        private void SetAvatars()
        {
            Avatars = AvatarsImpl;
        }

        protected VkViewModel(IVkAccount account) : base(account)
        {
            SetAvatars();
            SelectCurrent = new RelayCommand<string>(Select);
            SearchCommand = new RelayCommand<string>(Search);
        }

        public void SelectLastDialog()
        {
            Settings.Default.Reload();
            LastDialog = Settings.Default.LastDialog;
            if (!string.IsNullOrEmpty(LastDialog))
                Select(LastDialog);
        }

        private void Select(string dialog)
        {
            LastDialog = dialog;
            ChildViewModel = ProgressViewModel;

            DialogViewModel dialogVM;
            if (!_cachedViewModels.ContainsKey(dialog))
            {
                dialogVM = new DialogViewModel(Account, dialog, () => ChildViewModel = ProgressViewModel);
                _cachedViewModels[dialog] = dialogVM;
            }
            else
            {
                dialogVM = _cachedViewModels[dialog];
            }
            dialogVM.OnEnd = () => ChildViewModel = dialogVM;
            dialogVM.UpdateAsync(0);
        }

        private async void Search(string s)
        {
            await Task.Factory.StartNew(() =>
            {
                if (s == null)
                    return;
                if (string.IsNullOrWhiteSpace(s))
                {
                    if (Avatars.Count == AvatarsImpl.Count)
                    {
                        return;
                    }

                    Avatars = AvatarsImpl;
                    return;
                }

                Avatars = Avatars.Where(a =>
                    a.Key.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

    }
}