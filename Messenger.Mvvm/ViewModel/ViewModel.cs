using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace MvvmService.ViewModel
{
    public abstract class ViewModel<TModel, TKey, TChildViewModel> : ViewModelBase
    {
        public readonly Dictionary<TKey, TChildViewModel> _cachedViewModels =
            new Dictionary<TKey, TChildViewModel>();

        private ViewModelBase _childViewModel;
        public ViewModelBase ProgressViewModel = new ProgressViewModel();

        protected ViewModel(TModel account)
        {
            Account = account;
        }

        protected ViewModel()
        {
        }

        public ViewModelBase ChildViewModel
        {
            get { return _childViewModel; }
            set { Set(ref _childViewModel, value); }
        }

        protected TModel Account { get; set; }
    }
}