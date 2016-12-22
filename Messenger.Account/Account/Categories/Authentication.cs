using VkData.Account.Enums;
using VkData.Account.Interface;
using VkData.Account.Types;
using VkData.Helpers;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace VkData.Account.Categories
{
    public class Authentication : IVkAuthService
    {
        internal Authentication(IVkAccount account, IUserSettings user, bool useAsync)
        {
            account.ThrowIfNull();
            Account = account;
            Initialize(user);
            StartTracking(user, useAsync);
        }

        public
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams,
                    Photo, PhotoSize, StickerSize>
            Account { get; set; }

        public void Start()
        {
            Account.RaiseAuthenticated<Authentication>(UserSettings, Account.Callbacks.OnAuthenticationFailure, this);
        }

        private void Initialize(IUserSettings user)
        {
            user.ThrowIfNull();
            UserSettings = user;
        }

        private void StartTracking(IUserSettings user,
            bool useAsync)
        {
            Account.Authenticated += (sender, args) =>
            {
                Account.Callbacks?.OnAuthenticating?.Invoke();
                if (Account.IsAuthorized) return;
                Account.Authorized += Account.Callbacks?.OnAuthorized;
                Account.Authorization.Authorize(user, useAsync);
            };
        }

        #region IAuthenticationService support

        public IUserSettings UserSettings { get; set; }

        public bool TryAuthenticate(string password, string login)
        {
            if (!UserSettings.TryConfirm(password, login))
                return false;
            Account.RaiseAuthenticated(UserSettings, Account.Callbacks.OnAuthenticationFailure, this);
            return true;
        }

        #endregion
    }
}