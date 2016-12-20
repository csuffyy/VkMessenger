namespace VkData
{
    public delegate void NotificationHandler<TMessage>(object sender, NotificationArgs<TMessage> args);
}