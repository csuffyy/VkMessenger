using System.Collections.Generic;
using VkData.Account.Interface;

namespace MvvmService.ViewModel
{
    public class ChatsViewModel : VkViewModel
    {
        public ChatsViewModel(IVkAccount account) : base(account)
        {
        }


        protected override List<KeyValuePair<string, string>> AvatarsImpl => Account.Avatars.ChatList;
    }
}