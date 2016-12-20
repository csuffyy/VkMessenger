using System.Collections.Generic;

namespace VkData.Interface
{
    public interface IChats<TChat>
    {
        Dictionary<string, TChat> Dictionary { get; }
        List<KeyValuePair<string, TChat>> List { get; }
        void Update();
    }
}