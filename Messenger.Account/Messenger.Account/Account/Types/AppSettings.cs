using System;
using VkData.Helpers;
using VkData.Interface;

namespace VkData.Account.Types
{
    public sealed class AppSettings : IAppSettings
    {
        private string _userFolder;

        public AppSettings(string appDirectory, string userFolder)
        {
            BaseDirectory = appDirectory.JoinPath(userFolder);
            AppDirectory = appDirectory;
            UserFolder = userFolder;
        }


        public static string DefaultUserFolder { get; } = "DefaultUser";

        public string NotificationSoundPath => AppDirectory.JoinPath("notification.wav");

        public string AppDirectory { get; set; }

        public string UserFolder
        {
            get { return _userFolder; }
            set
            {
                if (value == _userFolder) return;
                _userFolder = value;

                if (value == DefaultUserFolder)
                    return;

                var newPath = BaseDirectory.RenameDirectoryPath(value);
                FileUtils.MergeDirectory(BaseDirectory, newPath);
                BaseDirectory = newPath;
            }
        }

        public string PhotoExtension { get; private set; }
        public string AvatarsPath => BaseDirectory.JoinPath("avatars");
        public string StoragePath => BaseDirectory.JoinPath("storage");
        public string LogPath => BaseDirectory.JoinPath("logs");
        public string PhotosPath => BaseDirectory.JoinPath("photos");
        public string StickersPath => BaseDirectory.JoinPath("stickers");
        public string ChatCoverPath => BaseDirectory.JoinPath("chat_covers");

        public string BaseDirectory { get; private set; }

        public static AppSettings Default() => Default(DefaultUserFolder);

        private static AppSettings Default(string userFolder)
        {
            return new AppSettings(Environment.CurrentDirectory, userFolder)
            {
                PhotoExtension = ".png"
            };
        }
    }
}