using System;
using System.Collections.Generic;

namespace VkData
{
    public class NotificationArgs<TMessage> : EventArgs
    {
        public NotificationArgs(IEnumerable<TMessage> newMessages)
        {
            NewMessages = newMessages;
        }

        public IEnumerable<TMessage> NewMessages { get; }
    }
}