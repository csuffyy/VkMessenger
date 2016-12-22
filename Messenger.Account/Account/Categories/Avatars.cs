using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VkData.Account.Enums;
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
    public class Avatars :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, PhotoSize>,
        IAvatars<Message>
    {
        public const string EmptyAvatar = "https://vk.com/images/camera_200.png";

        public Avatars(VkAccount Account) : base(Account)
        {
        }

        public async void DownloadAvatars()
        {
            FileUtils.CreateIfNotExist(Path);

            var friends = Account.Users.Friends;

            FriendsDictionary =
                friends.Select(item => new KeyValuePair<string, string>(item.Key, GetPath(Path, item)))
                    .ToDictionary(item => item.Key, item => item.Value);

            var valid = Directory.GetFiles(Path).Where(IsValidGDIPlusImage).ToList();


            foreach (
                var item in
                    FriendsDictionary.Values.Except(valid).Where(
                        s => s != GetEmptyAvatar(Path)))
            {
                var key = System.IO.Path.GetFileName(item)?.Replace(Account.AppSettings.PhotoExtension, string.Empty);

                if (key == null)
                    continue;
                 Account.Downloader.DownloadSyncronously(
                    friends[key].Photo100, item, PathOptions.Full, true);
            }
             Account.Downloader.DownloadSyncronously(EmptyAvatar, GetEmptyAvatar(Path), PathOptions.Full);

            foreach (var chat in Account.Chats.Dictionary)
                ChatDictionary[chat.Key] = await GetChatImage(chat.Key, 100);

            FriendsList = FriendsDictionary.ToList();
            ChatList = ChatDictionary.ToList();
        }

        public async Task<string> GetChatImage(string chatName, int size)
        {
            var coverPath = Account.AppSettings.ChatCoverPath;

            FileUtils.CreateIfNotExist(coverPath);

            var directory = coverPath.JoinPath(chatName);
            var path = $"{directory}{Account.AppSettings.PhotoExtension}";

            if (File.Exists(path))
            {
                if (IsValidGDIPlusImage(path))
                    return path;
                File.Delete(path);
            }

            var users = Account.Chats.Dictionary[chatName].Users;
            var images = new string[4];
            var indices = new int[4];
            var random = new Random();
            var range = users.Count/4;
            range = range == 0 ? 1 : range;
            indices[0] = random.Next(0, range);
            indices[1] = random.Next(indices[0] + 1, range*2);
            indices[2] = random.Next(indices[1] + 1, range*3);
            indices[3] = random.Next(indices[2] + 1, range*4 - 1);
           
            for (var i = 0; i < images.Length - 1; i++)
                images[i] = await Get(users[indices[i]]);

            images[3] = GetEmptyAvatar(Path);
            if (users.Count > 3)
                images[3] = await Get(users[3]);

            await Task.Factory.StartNew(
                () => CoupleImages(size, path, images, 0, s =>
                {
                    var key = System.IO.Path.GetFileName(s)?.Replace(Account.AppSettings.PhotoExtension, string.Empty);

                    var uri = key == "NoAvatar"
                        ? new Uri(EmptyAvatar)
                        : Account.Users.Friends[key].Photo100;

                    return
                        Account.Downloader.DownloadSyncronously(
                            uri, s, PathOptions.Full, true);
                }),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            return path;
        }


        /// <summary>
        ///     creates a square image from 4 images
        /// </summary>
        /// <param name="size">width and height</param>
        /// <param name="path">file path</param>
        /// <param name="images">represents 4 images</param>
        /// <param name="indent">indent bettween parts</param>
        /// <param name="requestAgain">empty image</param>
        protected static void CoupleImages(int size, string path, IList<string> images, int indent,
            Func<string, string> requestAgain)
        {
            for (var i = 0; i < images.Count; i++)
            {
                if (IsValidGDIPlusImage(images[i])) continue;
                images[i] = requestAgain(images[i]);
            }

            using (var bmp = new Bitmap(size, size))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    //part size
                    var s = size/2 - indent;
                    g.DrawImage(Image.FromFile(images[0]), 0, 0, s, s);
                    g.DrawImage(Image.FromFile(images[1]), s, 0, s, s);
                    g.DrawImage(Image.FromFile(images[2]), 0, s, s, s);
                    g.DrawImage(Image.FromFile(images[3]), s, s, s, s);
                    g.Save();
                }
                bmp.Save(path);
            }
        }

        public static bool IsValidGDIPlusImage(string filename)
        {
            try
            {
                using (var bmp = new Bitmap(filename))
                {
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region public properties

        public List<KeyValuePair<string, string>> FriendsList { get; private set; }
        public List<KeyValuePair<string, string>> ChatList { get; private set; }

        public Dictionary<string, string> FriendsDictionary { get; private set; }
        public Dictionary<string, string> ChatDictionary { get; } = new Dictionary<string, string>();

        public string Path => Account.AppSettings.AvatarsPath;

        #endregion

        #region private methods

        private string GetPath(string path, KeyValuePair<string, User> item)
            => GetPath(path, item.Key, item.Value.Photo100);

        private string GetPath(string path, string userName, Uri uri) => uri != null
            ? $@"{path}\{userName}{Account.AppSettings.PhotoExtension}"
            : GetEmptyAvatar(path);

        private string GetEmptyAvatar(string path)
            => $@"{path}\NoAvatar{Account.AppSettings.PhotoExtension}";

        #endregion

        #region public methods

        /// <summary>
        ///     returns local path to sender's avatar image
        /// </summary>
        /// <param name="message">VkNet.Model.Message</param>
        /// <returns>avatar path</returns>
        public Task<string> Get(Message message) => Get(Account.Users.GetFullUserName(message));

        /// <summary>
        ///     returns local path to sender's avatar image
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>avatar path</returns>
        public Task<string> Get(long userId) => Get(Account.Users.GetFullUserName(userId));

        /// <summary>
        ///     returns local path to user's or chat's avatar image
        /// </summary>
        /// <param name="dialogName">full name of user</param>
        /// <returns>avatar path</returns>
        public async Task<string> Get(string dialogName)
        {
            if (ChatDictionary.ContainsKey(dialogName))
                return ChatDictionary[dialogName];

            if (FriendsDictionary.ContainsKey(dialogName))
                return FriendsDictionary[dialogName];

            if (!Account.Storage.Users.ContainsKey(dialogName))
                return await GetChatImage(dialogName, 100);

            var user = Account.Storage.Users[dialogName];
            return await Account.Downloader.DownloadAsync(
                user.Photo100,
                GetPath(Path, dialogName, user.Photo100),
                PathOptions.Full);
        }

        #endregion
    }
}