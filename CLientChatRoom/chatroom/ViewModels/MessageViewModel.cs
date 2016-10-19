using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatroom.Classes;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.ViewModels
{
    public class MessageViewModel : BindBase
    {
        private Func<object, object> navigation;
        private Dictionary<UserModel, MessageModel> _messageBaseHistoryList;

        public MessageViewModel()
        {
            _messageBaseHistoryList = new Dictionary<UserModel, MessageModel>();
        }

        public MessageViewModel(Func<object, object> navigation) : this()
        {
            this.navigation = navigation;
        }

        public BusinessLogic BL
        {
            get { return _startup.BL; }
            set { _startup.BL = value; onPropertyChange("BL"); }
        }

        public Dictionary<UserModel, MessageModel> MessageHistoryList
        {
            get { return _messageBaseHistoryList; }
            set { setPropertyChange(ref _messageBaseHistoryList, value); }
        }

        public async void load()
        {
            // searching all common discussion
            var user_discussionAuthenticatedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = BL.BLSecurity.GetAuthenticatedUser().ID }, chatcommon.Enums.EOperator.AND);

            foreach (var user_discussion in user_discussionAuthenticatedUserList)
            {
                var messageList = (await BL.BLMessage.searchMessage(new Message { DiscussionId = user_discussion.DiscussionId }, chatcommon.Enums.EOperator.AND)).Where(x=>x.ID != user_discussion.UserId).OrderByDescending(x => x.Date).ToList();
                if (messageList.Count > 0)
                {
                    int nbCharToDisplay;
                    MessageModel lastMessage = messageList.Select(x => new MessageModel { Message = x }).First();
                    lastMessage.TxtContent = Utility.decodeBase64ToString(lastMessage.TxtContent);
                    if (lastMessage.TxtContent.Length > 30)
                        nbCharToDisplay = 30;
                    else
                        nbCharToDisplay = lastMessage.TxtContent.Length - 1;

                    lastMessage.TxtContent = lastMessage.TxtContent.Substring(0,nbCharToDisplay)+"...";
                    var userList = await BL.BLUser.searchUser(new User { ID = lastMessage.Message.UserId }, chatcommon.Enums.EOperator.AND);//).Where(x=>x.ID != BL.BLSecurity.GetAuthenticatedUser().ID).ToList();
                    if (userList.Count > 0)
                        MessageHistoryList = new Dictionary<UserModel, MessageModel> { { userList.Select(x => new UserModel { User = x }).First(), lastMessage } } ;//[userList.Select(x => new UserModel { User = x }).First()] = messageList.Select(x => new MessageModel { Message = x }).First();
                }
                
            }   
            
                     
        }
    }
}
