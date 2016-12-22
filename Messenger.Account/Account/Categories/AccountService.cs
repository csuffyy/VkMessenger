using VkData.Interface;

namespace VkData.Account.Categories
{
    public abstract class AccountService<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams,
        TPhoto, TStickerSize, TPhotoSize> :
            IAccountService
                <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize,
                    TPhotoSize>
    {
        protected AccountService(
            IAccount
                <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TPhotoSize,
                    TStickerSize> account)
        {
            Account = account;
        }

        public
            IAccount
                <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TPhotoSize,
                    TStickerSize> Account { get; }
    }
}