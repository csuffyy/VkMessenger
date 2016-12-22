namespace VkData.Interface
{
    public interface INotificationService<TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams,
        TPhoto, TStickerSize, TPhotoSize> :
            IAccountService
                <TMessage, TUser, TResponse, TApi, TPollSettings, TChat, TGetHistoryParams, TPhoto, TStickerSize,
                    TPhotoSize>
    {
        void StartTracking();
        void StopTracking();
    }
}