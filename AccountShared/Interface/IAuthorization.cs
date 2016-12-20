namespace VkData.Interface
{
    public interface IAuthorization
    {
        void Authorize(IUserSettings settings, bool isAsync);

        void AuthorizeUseTask(IUserSettings settings);

        void Authorize(IUserSettings settings);
    }
}