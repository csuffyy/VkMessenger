namespace VkData.Interface
{
    public interface IAppSettings
    {
        string NotificationSoundPath { get; }
        string AppDirectory { get; set; }
        string UserFolder { get; set; }
        string PhotoExtension { get; }
        string AvatarsPath { get; }
        string StoragePath { get; }
        string LogPath { get; }
        string PhotosPath { get; }
        string StickersPath { get; }
        string ChatCoverPath { get; }
        string BaseDirectory { get; }
    }
}