using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using VkData.Account.Categories;
using VkData.Account.Enums;
using VkData.Account.Extension;
using VkData.Account.Interface;
using VkData.Helpers;
using VkData.Interface;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace VkData.Account.Types
{
    public class MessageStrategy : IJsonParseStrategy<IVkAccount, JToken>
    {
        private readonly Dictionary<string, Func<IVkAccount, JToken, string, object>> AttachmentFactories = new Dictionary
            <string, Func<IVkAccount, JToken, string, object>>
        {
            {
                "photo", (account, token, attachment) =>
                    account.Photos.GetById(new List<string> {GetValue(token, attachment)}).First()
            },
            {
                "sticker", (account, token, attachment) =>
                {
                    var stickerId = Convert.ToInt32(GetValue(token, attachment));
                    return new Sticker
                    {
                        Photo64 = stickerId.GetUrl(StickerSize.Photo64),
                        Photo128 = stickerId.GetUrl(StickerSize.Photo128),
                        Photo256 = stickerId.GetUrl(StickerSize.Photo256),
                        Photo352 = stickerId.GetUrl(StickerSize.Photo352)
                    };
                }
            },
            {"video", (account, token, attachment) => null},
            {"audio", (account, token, attachment) => null},
            {"doc", (account, token, attachment) => null},
            {"link", (account, token, attachment) => null}
        };

        private readonly Regex attachmentMatchRegex = new Regex("attach\\d_type", RegexOptions.Compiled);

        public object Parse(JToken token, IVkAccount Account)
        {
            var dialogOwnerId = Convert.ToInt64(GetValue(token, 3));

            var jToken = token[7];

            var attachments = jToken.
                Where(CheckToken).
                Select((t, i) =>
                    GetAttachment(AttachmentFactories[GetValue(jToken, $"attach{i + 1}_type")]
                        (Account, jToken, $"attach{i + 1}"))).ToList();

            var fromId = VkNotifications.ParseFlags(2, 1, GetValue(token, 2)) != 1
                ? dialogOwnerId
                : Account.VkApi.UserId.Value;

            if (Chats.IsChatId(dialogOwnerId))
                fromId = Convert.ToInt64(GetValue(token, "from"));

            var dialogName = Account.Storage.DialogNames[dialogOwnerId];
            var updates =
                new KeyValuePair<string, Dialog<Message>>(dialogName, Dialog<Message>.Empty(dialogName, 0));

            updates.Value.Offsets[0].Value.Add(new Message
            {
                Date = Convert.ToInt64(GetValue(token, 4)).ToDateTime(),
                Body = GetValue(token, 6),
                Id = fromId,
                Attachments = new Collection<VkNet.Model.Attachments.Attachment>(attachments)
            });
            return updates;
        }

        private static VkNet.Model.Attachments.Attachment GetAttachment(object value) => new Attachment
        {
            Instance = value
        };

        public static string GetValue<T>(JToken token, T key)
        {
            if (!Equals(key, "from")) return token[key]?.ToString();
            var _token = token.Last.First;
            return _token.First?.ToString() ?? _token.ToString();
        }

        private bool CheckToken(JToken token) => attachmentMatchRegex.IsMatch(((JProperty) token).Name);
    }
}