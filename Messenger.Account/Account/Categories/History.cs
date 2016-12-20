using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using VkData.Account.Enums;
using VkData.Account.Extension;
using VkData.Account.Types;
using VkData.Helpers;
using VkData.Interface;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace VkData.Account.Categories
{
    public class History :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, Enums.PhotoSize>,
        IHistory<Message, LongPollServerSettings>
    {
        public const int DefaultMessagesCount = 20;
        public const int MaxMessagesCount = 200;
        public const int VkMessagesOffset = 20;

        public History(VkAccount Account) : base(Account)
        {
        }

        public Dictionary<string, Dialog<Message>> Dictionary
        {
            get { return Account.Storage.History; }
            set { Account.Storage.History = value; }
        }

        public LongPollServerSettings LongPollSettings
        {
            get
            {
                RefreshServer();
                Account.Storage.PollServerSettings.FromResponse(Account.LongPollServer);

                return Account.Storage.PollServerSettings;
            }
        }

        public void RefreshServer()
        {
            Account.LongPollServer = ((Func<LongPollServerResponse>)
                (() => Account.VkApi.Messages.GetLongPollServer(false, true))).
                Try(Account.Logger).
                Value;
        }

        public void Update(IReadOnlyCollection<KeyValuePair<string, Dialog<Message>>> pair)
        {
            Account.Storage.PollServerSettings.MaxMsgId =
                MaxMsgId(pair);
            foreach (var item in pair)
                Update(item.Value);
        }


        public void Update(Dialog<Message> dialog)
        {

            if (Dictionary.ContainsKey(dialog.Name))
                Dictionary[dialog.Name].Merge(dialog);
            else
                Dictionary[dialog.Name] = dialog;
        }

        public void GetUnreadHistory()
        {
            var currSettings = Account.Storage.PollServerSettings;

            if (currSettings.Empty || currSettings.TsOutdated)
            {
                currSettings = LongPollSettings;
                Account.Storage.Write(currSettings);
            }

            var history = GetLongPollHistory(currSettings);
            Update(history);
        }

        public void GetAllHistory()
        {
        }

        private Result<Dialog<Message>> GetHistory(string dialogName, MessagesGetHistoryParams @params,
            int? count = DefaultMessagesCount)
        {
            var offset = @params.Offset;
            offset.ThrowIfNull();

            var _offset = offset.ToNotNullable();

            if (Dictionary.ContainsKey(dialogName) 
                && Dictionary[dialogName].Offsets.ContainsKey(_offset))
                return new Result<Dialog<Message>>(Dictionary[dialogName], false);

            // if we have an key in a Dictionary 
            // an appropriate value is returned
            // so these lines are ignored
            @params.Count = count;
            var history =
                new Try<MessagesGetObject, TooManyRequestsException>(() => Account.VkApi.Messages.GetHistory(@params),
                    _ =>
                    {
                        var timer = new Timer(333);
                        timer.Start();
                    }, Account.Logger).Result.Value;
   var dialog =
                Dialog<Message>.GetDialog(dialogName, history.Messages.ToList(), _offset);

            Account.Storage.PollServerSettings.MaxMsgId =
                dialog.Offsets[_offset].Value.Select(m => m.Id).Max();

            //updates a local messages copy
             Update(dialog);

            return new Result<Dialog<Message>>(dialog);
        }

        public Result<Dialog<Message>> GetFriendHistory(string userName, long? offset = null,
            int count = DefaultMessagesCount)
        {
            return GetHistory(userName,
                new MessagesGetHistoryParams
                {
                    Offset = offset,
                    UserId = Account.Storage.Users[userName].Id
                }, count);
        }

        public Result<Dialog<Message>> GetChatHistory(string title, long? offset = null,
            int count = DefaultMessagesCount)
        {
            return GetHistory(title,
                new MessagesGetHistoryParams
                {
                    Offset = offset,
                    PeerId = Chats.ChatIdToPeerId(Account.Storage.Chats[title].Id)
                }, count);
        }

        public Result<Dialog<Message>> GetHistory(string recipient, long? offset, int? count = DefaultMessagesCount)
        {
            if (Account.Users.Friends.ContainsKey(recipient))
                return GetFriendHistory(recipient, offset);

            return Account.Chats.Dictionary.ContainsKey(recipient)
                ? GetChatHistory(recipient, offset)
                : new Result<Dialog<Message>>(
                    Dialog<Message>.Empty(recipient, offset.ToNotNullable()));
        }

        public List<Message> GetDialogs(int count = MaxMessagesCount)
        {
            var pars = new MessagesDialogsGetParams { Count = (uint)count };
            var result = new List<Message>();
            return ((Func<List<Message>>)
               (() =>
               {
                   result = Account.VkApi.Messages.GetDialogs(pars).Messages.Reverse().ToList();
                   return result;
               })).
               Try<List<Message>, WebException>(
                   e => Account.Callbacks.OnApiException(),
                   Account.Logger).
                     OnSuccess(() =>
                     {
                         if (result.Count != 0)
                         {
                             Account.Storage.PollServerSettings.MaxMsgId =
                             result.Select(m => m.Id).AsParallel().Max();
                         }
                         return result;
                     });
        }

        public Dictionary<string, Dialog<Message>> GetLongPollHistory(LongPollServerSettings server)
        {
            /* if (server.Empty || server.TsOutdated)
                 return new Dictionary<string, IDialog<Message>>();
                 */
            var @params = new MessagesGetLongPollHistoryParams
            {
                Ts = server.LocalTs,
                Pts = server.LocalPts,
                Fields = UsersFields.All
                //MaxMsgId = Account.Storage.PollServerSettings.MaxMsgId
            };
            var history =
                new Try<LongPollHistoryResponse, Exception>(
                    () => Account.VkApi.Messages.GetLongPollHistory(@params),
                   e => { }, Account.Logger).
                    Result.
                    Value;

            Account.Storage.PollServerSettings.LocalPts = history.NewPts;

            var ids = history.History.
                Where(upd => upd[0] == 4).
                ToDictionary(upd => upd[1]);

            return history.Messages.
                GroupBy(message => ids[message.Id.ToNotNullable()][3]).
                ToDictionary(
                    grp => Account.Storage.DialogNames[grp.Key],
                    grp =>
                        Dialog<Message>.GetDialog(Account.Storage.DialogNames[grp.Key], grp.Reverse().ToList(), 0));
        }

        public void SendMessage(string messageText, string recipient)
        {
            if (messageText.Length == 0)
                return;
            var @params = new MessagesSendParams
            {
                Message = messageText
            };
            if (Account.Storage.Users.ContainsKey(recipient))
            {
                @params.UserId = Convert.ToInt32(Account.Storage.Users[recipient].Id);
            }
            else if (Account.Storage.Chats.ContainsKey(recipient))
            {
                @params.PeerId = Chats.ChatIdToPeerId(Convert.ToInt32(
                    Account.Storage.Chats[recipient].Id));
            }
            var id = Account.Storage.PollServerSettings.MaxMsgId;

            Account.Storage.PollServerSettings.MaxMsgId =
                ((Func<long?>)(() => Account.VkApi.Messages.Send(@params))).
                    Try(Account.Logger).
                    OnFailure(() => id);
        }

        public string GetText(Message message)
        {
            var name = Account.Users.GetFullUserName(message);
            var body = message.GetBody(FormatOptions.UseTabs);
            return body.GetText(name);
        }

        private static long? MaxMsgId(IEnumerable<KeyValuePair<string, Dialog<Message>>> obj)
        {
            return obj.SelectMany(p => p.Value.Projection)
                .Select(m => m.Id).AsParallel().Max();
        }
    }
}