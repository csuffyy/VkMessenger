using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using VkData.Account.Enums;
using VkData.Account.Types;
using VkNet.Model;

namespace VkData.Account.Extension
{
    public static class VkNetExtensions
    {
        private const string Break = "<br>";
        private const string Quot = "&quot";

        public static string GetFullName(this User user)
        {
            return $"{user.FirstName} {user.LastName}";
        }

        public static IEnumerable<object> GetAttachments(this Message message)
            => message.Attachments.Select(a => a.Instance);

        public static Attachment Decorate(this VkNet.Model.Attachments.Attachment attachment)
        {
            return new Attachment
            {
                Instance = attachment.Instance
            };
        }

        public static string GetResponseString(this WebRequest request)
        {
            using (var response = request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            {
                Debug.Assert(dataStream != null, "dataStream != null");
                var reader = new StreamReader(dataStream);
                return reader.ReadToEnd();
            }
        }

        public static string GetBody(this Message message, FormatOptions options)
            => message.Body.Replace(Quot, @"""").Replace(Break, options == FormatOptions.UseTabs ? "\n\t" : "\n");

        public static string GetTime(this Message message) => message.Date?.AddHours(-1.0).ToShortTimeString();

        public static string GetUrl(this int stickerId, StickerSize size)
            => $"https://vk.com/images/stickers/{stickerId}/{(int) size}.png";

        public static long GetValidUserId(this Message message)
        {
            if (message.FromId == null
                && message.UserId == null)
                return message.Id.Value;

            if (message.FromId == null && message.UserId != null)
            {
                return message.UserId.Value;
            }
            return message.FromId.Value;
        }
    }
}