using System;
using System.Collections.Generic;
using System.Linq;
using VkData.Account.Enums;
using VkData.Account.Extension;
using VkData.Account.Types;
using VkData.Helpers;
using VkData.Interface;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using PhotoSize = VkData.Account.Enums.PhotoSize;

namespace VkData.Account.Categories
{
    public class Users :
        AccountService
            <Message, User, LongPollServerResponse, VkApi, LongPollServerSettings, Chat, MessagesGetHistoryParams, Photo, StickerSize, PhotoSize>,
        IUsers<Message, User>
    {
        public Users(VkAccount Account) : base(Account)
        {
        }

        private User CurrentUser =>
            ((Func<User>) (
                () => Account.VkApi.Users.Get(Account.VkApi.UserId.ToNotNullable()))).
                Try(Account.Logger);

        public string Current
        {
            get
            {
                var id = Account.VkApi.UserId.ToNotNullable();

                if (Account.Storage.DialogNames != null
                    && Account.Storage.DialogNames.ContainsKey(id))
                    return Account.Storage.DialogNames[id];

                return CurrentUser.GetFullName();
            }
        }


        public Dictionary<string, User> Friends
        {
            get
            {
                if (Account.Storage.Users.Count != 0)
                    return Account.Storage.Users;

                var @params = new FriendsGetParams
                {
                    Fields = ProfileFields.All
                };

                var friends =
                    ((Func<Dictionary<string, User>>) (
                        () =>
                            Account.VkApi.Friends.Get(@params).
                                GroupBy(user => user.GetFullName()).
                                Select(group => group.First()).
                                ToDictionary(u => u.GetFullName()))).Try(Account.Logger);

                var current = CurrentUser;

                friends.Value[current.GetFullName()] = current;

                return friends;
            }
        }


        public void Update()
        {
            var friends = Account.Users.Friends;
            /*
            foreach (var item in friends)
            {
                var user = item.Value;
                Account.Storage.UserIds[user.Id] = user;
                Account.Storage.DialogNames[user.Id] = item.Key;
            }
            */
            if (Account.Storage.UserIds.Count == 0)
            {
                foreach (var user in friends.Select(item => item.Value))
                {
                    Account.Storage.UserIds[user.Id] = user;
                }
            }
            if (Account.Storage.DialogNames.Count == 0)
            {
                foreach (var item in friends)
                {
                    var user = item.Value;
                    Account.Storage.DialogNames[user.Id] = item.Key;
                }
            }

            if (Account.Storage.Users.Count == 0)
                Account.Storage.Users = friends;
        }

        public void Update(IEnumerable<User> users)
        {
            foreach (var item in users)
            {
                Update(item);
            }
        }

      

        public void Update(Dialog<Message> obj)
        {
            //users which are not stored in dictionary
            var ids = obj.Messages.SelectMany(p => p.Value).Select(message => message.FromId.ToNotNullable()).ToList();
            var users = Account.Users.RequireNotCachedUsers(ids);
            Update(users);
        }

        public void Update(IEnumerable<KeyValuePair<string, Dialog<Message>>> updates)
        {
            foreach (var item in updates)
                Update(item.Value);
        }


        public string GetFullUserName(long? id)
            => Account.Storage.UserIds[id.ToNotNullable()].GetFullName();

        public IEnumerable<User> RequireNotCachedUsers(IEnumerable<long> userIds)
        {
            var notCached =
                userIds.
                    Where(id => !Account.Storage.DialogNames.ContainsKey(id)).
                    Distinct().
                    ToList();

            return notCached.Count != 0
                ? Account.VkApi.Users.Get(notCached, ProfileFields.All).ToList()
                : new List<User>();
        }

        public string GetFullUserName(long userId)
        {
            if (Account.Storage.UserIds.ContainsKey(userId))
                return Account.Storage.UserIds[userId].GetFullName();

            var user = Account.VkApi.Users.Get(userId, ProfileFields.All);
            Update(user);
            return Account.Storage.UserIds[userId].GetFullName();
        }

        public string GetFullUserName(Message message)
        {
            return GetFullUserName(GetValidUserId(message));
        }

        private void Update(User item)
        {
            Account.Storage.UserIds[item.Id] = item;
            Account.Storage.Users[item.GetFullName()] = item;
            Account.Storage.DialogNames[item.Id] = item.GetFullName();
        }

        public static long GetValidUserId(Message message)
        {
            if (message.FromId == null
                && message.UserId == null)
                return message.Id.ToNotNullable();

            if (message.UserId != null)
                return message.UserId.Value;

            return message.FromId.ToNotNullable();
        }
    }
}