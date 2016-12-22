using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class Photos :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, PhotoSize>,
        IPhotos<Photo, PhotoSize>
    {
        public Dictionary<PhotoSize, Func<Photo, Uri>> _photoMappings = new Dictionary<PhotoSize, Func<Photo, Uri>>
        {
            {PhotoSize.Photo75, photo => photo.Photo75},
            {PhotoSize.Photo130, photo => photo.Photo130},
            {PhotoSize.Photo604, photo => photo.Photo604},
            {PhotoSize.Photo807, photo => photo.Photo807},
            {PhotoSize.Photo1280, photo => photo.Photo1280},
            {PhotoSize.Photo2560, photo => photo.Photo2560}
        };

        public Photos(VkAccount account) : base(account)
        {
        }

        public string Path => Account.AppSettings.PhotosPath;

        public async Task<List<string>> GetPathById(IEnumerable<Photo> photos, PhotoSize size)
        {
            var tasks = photos.ToList();

            var list = new List<string>();

            foreach (var item in tasks)
            {
                list.Add(await Account.Downloader.DownloadAsync(_photoMappings[size](item), GetPath(size, item), PathOptions.Full));
            }

            return list;
        }

        public ReadOnlyCollection<Photo> GetById(IEnumerable<string> ids)
            => ((Func<ReadOnlyCollection<Photo>>) (() => Account.VkApi.Photo.GetById(ids))).
                Try(Account.Logger);

        public Task<List<string>> GetPathByAttachments(IEnumerable<object> attachments, PhotoSize size)
            => GetPathById(attachments.OfType<Photo>(), size);

        private string GetPath(PhotoSize size, MediaAttachment photo)
            => $@"{Path}\{size}\{photo.Id}{Account.AppSettings.PhotoExtension}";
    }
}