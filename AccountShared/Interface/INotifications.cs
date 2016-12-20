using VkData.Account.Types;
using VkNet.Model;

namespace VkData.Interface
{
    public interface INotifications<TMessage, in TResponse> 
    {
        bool IsStarted { get; }
        ObservableConcurrentDictionary<string, Dialog<Message>> Notifications { get; set; }
        void WaitForUpdates(TResponse response, ref ulong newTs);
        void Start();
        void Cancel();
    }
}