namespace MvvmService
{
    public interface IWPFAppSettings
    {
        string Password { get; set; }
        string Login { get; set; }
        ulong VkAppId { get; }
        string LastDialog { get; set; }

        void Upgrade();
        void Save();
        void Reset();
        void Reload();
    }
}