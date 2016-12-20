using System.Collections.Generic;

namespace VkData.Interface
{
    public interface IDialog<TMessage>
    {
        Dictionary<long , List<TMessage>>  Messages { get; }
        string Name { get; }
        void Merge(IDialog<TMessage> other);
    }
}