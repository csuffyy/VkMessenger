namespace VkData.Interface
{
    public interface IAuthenticationService<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto,  TStickerSize,  TPhotoSize> :
        IAccountService<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize ,TPhotoSize>
    {
        IUserSettings UserSettings { get; set; }
        bool TryAuthenticate(string password, string login);

        /// <summary>
        ///     Starts authentication, and, if succesful, starts authorization.
        ///     After authorization is complete all corresponding callbacks
        ///     execute
        /// </summary>
        void Start();
    }
}