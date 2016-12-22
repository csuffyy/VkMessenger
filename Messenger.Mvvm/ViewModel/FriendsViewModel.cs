using System.Collections.Generic;
using VkData.Account.Interface;

namespace MvvmService.ViewModel
{
    public class FriendsViewModel : VkViewModel
    {
        public FriendsViewModel(IVkAccount account) : base(account)
        {
        }

        protected override List<KeyValuePair<string, string>> AvatarsImpl => Account.Avatars.FriendsList;
    }
}