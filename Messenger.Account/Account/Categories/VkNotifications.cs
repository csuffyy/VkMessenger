using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VkData.Account.Enums;
using VkData.Account.Interface;
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
    public class VkNotifications :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo,
                StickerSize, PhotoSize>,
        INotifications<Message, LongPollServerResponse>
    {
        private readonly Dictionary<string, IJsonParseStrategy<IVkAccount, JToken>>
            _strategies = new Dictionary<string, IJsonParseStrategy<IVkAccount, JToken>>
            {
                {"4", new MessageStrategy()}
            };

        public ulong NewTs;

        public VkNotifications(
            IAccount
                <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams,
                    Photo, PhotoSize, StickerSize>
                Account) : base(Account)
        {
        }

        public ObservableConcurrentDictionary<string, Dialog<Message>> Notifications { get; set; } =
            new ObservableConcurrentDictionary<string, Dialog<Message>>();

        public bool IsStarted { get; private set; }

        public void TrackUpdates(LongPollServerResponse response, ref ulong newTs)
        {
            if (response == null)
            {
                Account.History.RefreshServer();
                response = Account.LongPollServer;
            }
            var url = GetUrl(response, newTs);
            var jObject = new HttpQuery<JObject>(url,
                JObject.Parse, 25000, Account.Logger).Result.Value;
            if (jObject.Count == 0) return;
            newTs = Convert.ToUInt64(jObject.GetValue("ts"));
            var parseResult = ParseJObject(jObject);
            var updates = parseResult.
                OfType<KeyValuePair<string, Dialog<Message>>>().
                ToList();

            UpdateCaches(Distinct(updates));
        }

        private Dictionary<string, Dialog<Message>> Distinct(List<KeyValuePair<string, Dialog<Message>>> updates)
        {
            var distinct = new Dictionary<string, Dialog<Message>>();
            foreach (var item in updates)
            {
                if (Account.Storage.History.ContainsKey(item.Key))
                {
                    if (item.Value.Offsets[0].Value[0].Id
                        == Account.Storage.History[item.Key].Offsets[0].Value[0].Id)
                    {
                        continue;
                    }
                }
                distinct.Add(item.Key, item.Value);
            }
            return distinct;
        }

        public void Start()
        {
            Task.Factory.StartNew(
                () =>
                {
                    if (IsStarted)
                        return;

                    IsStarted = true;
                    do
                    {
                        TrackUpdates(Account.LongPollServer, ref NewTs);
                    } while (!Account.CancellationTokenSource.Token.IsCancellationRequested);
                    Account.Logger.Log("Updates stopped");
                },
                Account.CancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public void Cancel() => Account.CancellationTokenSource.Cancel();

        private IEnumerable<object> ParseJObject(JObject responseJson)
        {
            if (responseJson == null)
                throw new ArgumentNullException(nameof(responseJson));

            var jToken = responseJson["updates"];
            return jToken?.Select(ParseJtoken) ?? new List<object>();
        }

        private object ParseJtoken(JToken token)
        {
            var flag = GetFlag(token);
            return _strategies.ContainsKey(flag)
                ? _strategies[flag].Parse(token, (IVkAccount) Account)
                : null;
        }

        private static string GetFlag(JToken token) => token[0].ToString();

        private void UpdateCaches(IEnumerable<KeyValuePair<string, Dialog<Message>>> updates)
        {
            Notifications.Clear();

            var upd = updates.ToList();
            Account.History.Update(upd);
            Account.Users.Update(upd);
            Notifications.Add(upd);
        }

        private static string GetUrl(LongPollServerResponse response, ulong newTs)
            => $"http://{response.Server}?act=a_check&key={response.Key}&ts={newTs}&wait=25&mode=2";

        public static long ParseFlags(int mask, int shift, string flag)
        {
            long flags = Convert.ToInt32(flag);
            return (flags & mask) >> shift;
        }
    }
}