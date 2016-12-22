using System.Collections.Generic;
using System.Linq;
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
    public class Chats :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, PhotoSize>,
        IChats<Chat>
    {
        private const int VkChatFlag = (int) 2e9;


        public Chats(VkAccount Account) : base(Account)
        {
        }

        private IEnumerable<Chat> GetChatsInternal
        {
            get
            {
                if (Account.Storage.Chats.Count != 0)
                    return Account.Storage.Chats.Select(p => p.Value);

                var chatIds = Account.History.GetDialogs(History.MaxMessagesLimit).
                    Where(message => message.ChatId != null).
                    Select(message => message.ChatId.Value).
                    ToList();

                if (!chatIds.Any())
                    return new List<Chat>();

                var _chats = Account.VkApi.Messages.GetChat(chatIds).
                    GroupBy(chat => chat.Title).
                    Select(group => @group.First()).
                    Where(c => c.Users.Count != 0);
                var list = _chats.ToList();
                return list;
            }
        }

        public Dictionary<string, Chat> Dictionary
        {
            get { return GetChatsInternal.ToDictionary(chat => chat.Title, chat => chat); }
        }

        public List<KeyValuePair<string, Chat>> List =>
            Dictionary.ToList();

        public void Update()
        {
            foreach (var item in List)
            {
                var chat = item.Value;
                Account.Storage.Chats[chat.Title] = chat;
                Account.Storage.ChatIds[chat.Id] = chat;
                Account.Storage.DialogNames[ChatIdToPeerId(chat.Id)] = item.Key;
            }
        }

        public static bool IsChatId(long id) => id > VkChatFlag;
        public static long ChatIdToPeerId(long chatId)
        {
            return chatId + VkChatFlag;
        }
    }
}