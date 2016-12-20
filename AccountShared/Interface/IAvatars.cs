using System.Collections.Generic;
using System.Threading.Tasks;

namespace VkData.Interface
{
    public interface IAvatars<in TMessage>
    {
        List<KeyValuePair<string, string>> FriendsList { get; }
        List<KeyValuePair<string, string>> ChatList { get; }
        Dictionary<string, string> FriendsDictionary { get; }
        Dictionary<string, string> ChatDictionary { get; }
        string Path { get; }
        Task<string> GetChatImage(string chatName, int size);
        void DownloadAvatars();

        /// <summary>
        ///     returns local path to sender's avatar image
        /// </summary>
        /// <param name="message">VkNet.Model.Message</param>
        /// <returns>avatar path</returns>
        Task<string> Get(TMessage message);

        /// <summary>
        ///     returns local path to sender's avatar image
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>avatar path</returns>
        Task<string> Get(long userId);

        /// <summary>
        ///     returns local path to user's or chat's avatar image
        /// </summary>
        /// <param name="dialogName">full name of user</param>
        /// <returns>avatar path</returns>
        Task<string> Get(string dialogName);
    }
}