using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkData.Account.Enums;
using VkData.Account.Extension;
using VkData.Account.Types;
using VkData.Interface;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace VkData.Account.Categories
{
    public class Stickers :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, PhotoSize>,
        IStickers<Message, StickerSize>
    {
        private readonly Dictionary<StickerSize, Func<Sticker, string>> _stickerMappings = new Dictionary
            <StickerSize, Func<Sticker, string>>
        {
            {StickerSize.Photo64, sticker => sticker.Photo64},
            {StickerSize.Photo128, sticker => sticker.Photo128},
            {StickerSize.Photo256, sticker => sticker.Photo256},
            {StickerSize.Photo352, sticker => sticker.Photo352}
        };

        public Stickers(VkAccount account) : base(account)
        {
        }

        public string Path => Account.AppSettings.StickersPath;


        public Task<string> Get(IEnumerable<object> attachments, StickerSize size)
        {
            var stickers = attachments.OfType<Sticker>().ToList();
            return stickers.Count == 0 ? null : Get(size, stickers.First());
        }

        public Task<string> Get(Message message, StickerSize size)
        {
            var sticker = message.Attachments.Count != 0
                ? message.GetAttachments().First() as Sticker
                : null;
            return Get(size, sticker);
        }

        public Task<string> Get(string url) => Account.Downloader.DownloadAsync(url, GetPath(url), PathOptions.Full);

        public string GetSize(string url, string extension) => url?.Split('/').Last().Replace(extension, string.Empty);

        public string GetId(string url)
        {
            var parts = url.Split('/');
            return parts[parts.Length - 2];
        }

        private Task<string> Get(StickerSize size, Sticker sticker)
            => sticker != null ? Get(_stickerMappings[size](sticker)) : null;

        private string GetPath(string url)
            =>
                $@"{Path}\{GetSize(url, Account.AppSettings.PhotoExtension)}\{GetId(url)}{Account.AppSettings
                    .PhotoExtension}";
    }
}