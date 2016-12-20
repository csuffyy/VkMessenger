using System.Collections.Generic;
using VkData.Account.Types;

namespace VkData.Interface
{
    public interface IUsers<TMessage, TUser> 
    {
        string Current { get; }
        Dictionary<string, TUser> Friends { get; }
        void Update();
        void Update(IEnumerable<TUser> users);
        void Update(Dialog<TMessage> obj);
        void Update(IEnumerable<KeyValuePair<string, Dialog<TMessage>>> updates);
        string GetFullUserName(long? id);
        IEnumerable<TUser> RequireNotCachedUsers(IEnumerable<long> userIds);
        string GetFullUserName(long userId);
        string GetFullUserName(TMessage message);
    }
}