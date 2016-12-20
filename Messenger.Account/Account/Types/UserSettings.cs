using System;
using VkData.Helpers;
using VkData.Interface;

namespace VkData.Account.Types
{
    public class UserSettings : IUserSettings
    {
        public UserSettings(Func<string> getSecurePassword, Func<string> getSecureLogin, Func<ulong> getAppId, Func<string> getSmsCode, Func<Exception, string> getCaptcha, Action<string> setPassword, Action<string> setLogin, Action saveSettings)
        {
            if (getAppId == null)
                throw new ArgumentNullException(nameof(getAppId));

            if (getSecureLogin == null)
                throw new ArgumentNullException(nameof(getSecureLogin));

            if (getSecurePassword == null)
                throw new ArgumentNullException(nameof(getSecurePassword));

            GetAppId = getAppId;
            GetSecurePassword = getSecurePassword;
            GetSecureLogin = getSecureLogin;
            SaveSettings = saveSettings;
            SetLogin = setLogin;
            SetPassword = setPassword;
            GetSmsCode = getSmsCode;
            CaptchaHandler = getCaptcha;
        }

        public Func<string> GetSmsCode { get; }

        public Func<Exception, string> CaptchaHandler { get; }

        public Func<ulong> GetAppId { get; }


        public bool Empty => GetSecureLogin().Length == 0 || GetSecurePassword().Length == 0;

        public Func<string> GetSecurePassword { get; }

        public Func<string> GetSecureLogin { get; }

        public Action SaveSettings { get; }

        public Action<string> SetLogin { get; }

        public Action<string> SetPassword { get; }

        public void Save(string _login, string _password)
        {
            SetLogin(_login);
            SetPassword(_password);
            SaveSettings();
        }

        public void Confirm(string password, string login)
        {
            var _login = login.Protect();
            var _password = password.Protect();
            Save(_login, _password);
        }


        public bool TryConfirm(string password, string login)
        {
            var utils = new EmailUtilities();
            if (password.Length == 0) return false;
            if (!utils.IsValidLogin(login)) return false;
            Confirm(password, login);
            return true;
        }
    }
}