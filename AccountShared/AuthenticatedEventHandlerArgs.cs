using VkData.Interface;

namespace VkData
{
    public class AuthenticatedEventHandlerArgs
    {
        public AuthenticatedEventHandlerArgs(IUserSettings settings)
        {
            Settings = settings;
        }

        public IUserSettings Settings { get; }
    }
}