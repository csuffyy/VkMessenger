using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkData.Account.Types;
using VkData.Helpers;
using VkData.Interface;
using VkNet.Model;

namespace VkData.Account.Categories
{
    public class Storage : IStorage<Message, LongPollServerSettings, Chat, User>
    {
        public Storage(ISerializer serializer, ILogger Logger, Action refreshAccount) : this(refreshAccount)
        {
            this.Logger = Logger;
            Writer = new CacheWriter(serializer, Logger);
        }

        private Storage(Action refreshAccount)
        {
            RefreshAccount = refreshAccount;
        }

        public Storage(string path, ISerializer serializer, ILogger logger, Action refreshAccount)
            : this(refreshAccount)
        {
            Logger = logger;
            Writer = new CacheWriter(path, serializer, logger);
        }

        public ILogger Logger { get; set; }

        protected CacheWriter Writer { get; }
        private Dictionary<object, Action> _writeMappingObject { get; set; }
        private Dictionary<string, Action> _writeMappingString { get; set; }
        public Action RefreshAccount { get; set; }

        public bool WriteEnabled { get; set; } = true;

        public string Path
        {
            get { return Writer.Path; }
            set { Writer.Path = value; }
        }

        public Dictionary<string, Chat> Chats { get; set; }
        public Dictionary<string, User> Users { get; set; }
        public Dictionary<string, Dialog<Message>> History { get; set; }
        public Dictionary<long, User> UserIds { get; set; }
        public Dictionary<long, Chat> ChatIds { get; set; }
        public Dictionary<long, string> DialogNames { get; set; }
        public LongPollServerSettings PollServerSettings { get; set; }

        public void Clear(params string[] files) => Writer.Delete(files);

        public void ClearAll()
        {
            Clear("history",
                "userIds",
                "users",
                "chatIds",
                "chats",
                "dialogNames",
                "longPollServerSettings");
            PollServerSettings = new LongPollServerSettings();

            UserIds = new Dictionary<long, User>();
            ChatIds = new Dictionary<long, Chat>();
            Chats = new Dictionary<string, Chat>();
            DialogNames = new Dictionary<long, string>();
            History = new Dictionary<string, Dialog<Message>>();
            Users = new Dictionary<string, User>();
            // if you dont refresh an account
            // you'll get problems in your account
            // but it isn't exacly the problem
            // of storage, but account so we inject
            // an action, that refreshes account
            RefreshAccount?.Invoke();
        }

        public void LoadAll()
        {
            var item = default(KeyValuePair<string, Dialog<Message>>);
            Parallel.Invoke(
                () => { History = Writer.LoadCollection(item, History, "history"); },
                () => UserIds = Writer.Load(UserIds, "userIds"),
                () => Users = Writer.Load(Users, "users"),
                () => ChatIds = Writer.Load(ChatIds, "chatIds"),
                () => Chats = Writer.Load(Chats, "chats"),
                () => DialogNames = Writer.Load(DialogNames, "dialogNames"),
                () => PollServerSettings = Writer.Load(PollServerSettings, "longPollServerSettings"));
            CreateMappingObject();
            CreateMappingString();
        }

        /*
        public void Write()
        {
            Writer
        }*/

        public void Write(string propertyName)
        {
          if(!WriteEnabled)
              return;
            if (_writeMappingString.ContainsKey(propertyName))
                _writeMappingString[propertyName]?.Invoke();
        }

        public void Write(object property)
        {
          if(!WriteEnabled)
              return;
            if (_writeMappingObject.ContainsKey(property))
                _writeMappingObject[property]?.Invoke();
        }

        public void WriteAll()
        {
          if(!WriteEnabled)
              return;
/*
            Writer.Write(DialogNames, "dialogNames");
            Writer.Write(UserIds, "userIds");
            Writer.Write(Users, "users");
            Writer.Write(ChatIds, "chatIds");
            Writer.Write(Chats, "chats");
            Writer.Write(PollServerSettings, "longPollServerSettings");
            Writer.WriteCollection(History, item => item.Key, "history");*/
            foreach (var item in _writeMappingString)
                item.Value();
        }

        private void CreateMappingString()
        {
            _writeMappingString
                = new Dictionary<string, Action>
                {
                    {
                        "chats", () =>
                            Writer.Write(Chats, "chats")
                    },
                    {
                        "users",
                        () => Writer.Write(Users, "users")
                    },
                    {"dialogNames", () => Writer.Write(DialogNames, "dialogNames")},
                    {"userIds", () => Writer.Write(UserIds, "userIds")},
                    {"chatIds", () => Writer.Write(ChatIds, "chatIds")},
                    {"longPollServerSettings", () => Writer.Write(PollServerSettings, "longPollServerSettings")},
                    {"history", () => Writer.WriteCollection(History, item => item.Key, "history")}
                };
        }

        private void CreateMappingObject()
        {
            _writeMappingObject
                = new Dictionary<object, Action>
                {
                    {Chats, () => Writer.Write(Chats, "chats")},
                    {Users, () => Writer.Write(Users, "users")},
                    {DialogNames, () => Writer.Write(DialogNames, "dialogNames")},
                    {UserIds, () => Writer.Write(UserIds, "userIds")},
                    {ChatIds, () => Writer.Write(ChatIds, "chatIds")},
                    {PollServerSettings, () => Writer.Write(PollServerSettings, "longPollServerSettings")},
                    {History, () => Writer.WriteCollection(History, item => item.Key, "history")}
                };
        }
    }
}
