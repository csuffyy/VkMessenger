namespace VkData.Interface
{
    public interface IAccountService<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto,  TStickerSize ,  TPhotoSize>
    {
        IAccount<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TPhotoSize, TStickerSize> Account { get; }
    }
}