using System;

namespace VkData.Interface
{
    public interface INotificationProvider<in TNotification>
    {
        Action<TNotification> Add { get; }
    }
}