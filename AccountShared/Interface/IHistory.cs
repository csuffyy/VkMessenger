using System.Collections.Generic;
using VkData.Helpers;
using VkNet.Model;

namespace VkData.Interface
{
    public interface IHistory<TMessage, TPollSettings>
    {
        Dictionary<string, Dialog<Message>> Dictionary { get; set; }
        TPollSettings LongPollSettings { get; }

        void Update(IReadOnlyCollection<KeyValuePair<string, Dialog<TMessage>>> pair);
        void Update(Dialog<TMessage> dialog);
        void GetUnreadHistory();
        void GetAllHistory();
        Result<Dialog<TMessage>> GetFriendHistory(string userName, long? offset, int count);
        Result<Dialog<TMessage>> GetChatHistory(string title, long? offset, int count);
        Result<Dialog<TMessage>> GetHistory(string recipient, long? offset, int? count);
        List<TMessage> GetDialogs(int count);
        Dictionary<string, Dialog<TMessage>> GetLongPollHistory(TPollSettings server);
        void SendMessage(string messageText, string recipient);
        string GetText(TMessage message);
        void RefreshServer();
    }
}