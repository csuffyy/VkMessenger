using System;
using VkData.Interface;

namespace VkData
{
    public abstract class AccountCallbacks<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams,
        TPhoto, TStickerSize, TPhotoSize>
    {
        protected AccountCallbacks(Action onAuthenticating,
            Action
                <
                    IAuthenticationService
                        <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize,
                            TPhotoSize>> onAuthenticationFailure,
            Action onAuthorized, Action<Exception> onAuthorizationException, Action onApiException)
        {
            OnAuthenticating = onAuthenticating;
            OnAuthorized = onAuthorized;
            OnAuthorizationException = onAuthorizationException;
            OnApiException = onApiException;
            OnAuthenticationFailure = onAuthenticationFailure;
        }

        protected AccountCallbacks(
            AccountCallbacks
                <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize,
                    TPhotoSize> callbacks)
            : this(callbacks.OnAuthenticating,
                callbacks.OnAuthenticationFailure, callbacks.OnAuthorized, callbacks.OnAuthorizationException,
                callbacks.OnApiException)
        {
        }

        public Action<Exception> OnAuthorizationException { get; }
        public Action OnAuthenticating { get; set; }

        public
            Action
                <
                    IAuthenticationService
                        <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize,
                            TPhotoSize>> OnAuthenticationFailure { get; set; }

        public Action OnAuthorized { get; }
        public Action OnApiException { get; }
    }
}