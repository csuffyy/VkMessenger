using System;
using System.Collections.Generic;
using VkNet.Model;

namespace VkData.Interface
{
    public interface IStorage<TMessage, TPollSettings, TChat, TUser> : IFileService
    {
        Action RefreshAccount { get; set; }
        Dictionary<long, TChat> ChatIds { get; set; }
        Dictionary<string, TChat> Chats { get; set; }
        Dictionary<long, string> DialogNames { get; set; }
        Dictionary<string, Dialog<Message>> History { get; set; }
        TPollSettings PollServerSettings { get; set; }
        Dictionary<long, TUser> UserIds { get; set; }
        Dictionary<string, TUser> Users { get; set; }
        void Clear(params string[] files);
        void ClearAll();
        void LoadAll();
        void WriteAll();
        /// <summary>
        /// If property is present in storage it's being saved.
        /// </summary>
        /// <param name="propertyName"></param>
        void Write(string propertyName);
        /// <summary>
        /// If passed object is present in storage it's being saved.
        /// </summary>
        /// <param name="property">passed object</param>
        void Write(object property);
    }
}