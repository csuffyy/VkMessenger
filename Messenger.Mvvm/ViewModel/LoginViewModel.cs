using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using VkData.Account.Categories;

namespace MvvmService.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IWPFAppResources _appResources =
            SimpleIoc.Default.GetInstance<IWPFAppResources>();

        private string _errorMessage;
        private string _login;
        private ObservableCollection<string> _recentUsers;

        public LoginViewModel()
        {
        }

        public LoginViewModel(Authentication service)
        {
            Service = service;
            RecentUsers = new ObservableCollection<string> {};
        }

        public Authentication Service { get; set; }

        public string Login
        {
            get { return _login; }
            set { Set(ref _login, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        public ObservableCollection<string> RecentUsers
        {
            get { return _recentUsers; }
            set { Set(ref _recentUsers, value); }
        }

        public void ConfirmCredentials(string password, string login)
        {
            ErrorMessage = string.Empty;

            if (password.Length == 0)
            {
                ErrorMessage = _appResources.TooShortPasswordMessage;
                return;
            }

            if (Service.TryAuthenticate(password, login))
                return;

            ErrorMessage = _appResources.IncorrectLoginMessage;
            Login = string.Empty;
        }
    }
}