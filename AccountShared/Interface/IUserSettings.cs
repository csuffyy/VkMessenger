using System;

namespace VkData.Interface
{
    public interface IUserSettings
    {
        Func<string> GetSecurePassword { get; }
        Func<string> GetSecureLogin { get; }
        Func<ulong> GetAppId { get; }
        bool Empty { get; }
        Action SaveSettings { get; }
        Action<string> SetLogin { get; }
        Action<string> SetPassword { get; }
        Func<Exception, string> CaptchaHandler { get; }
        void Save(string _login, string _password);
        void Confirm(string password, string login);
        bool TryConfirm(string password, string login);
    }
}