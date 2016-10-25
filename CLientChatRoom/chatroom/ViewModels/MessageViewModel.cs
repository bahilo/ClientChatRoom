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

        public User AuthenticatedUser
        {
            get { return BL.BLSecurity.GetAuthenticatedUser(); }
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
            var user_discussionAuthenticatedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = AuthenticatedUser.ID }, chatcommon.Enums.EOperator.AND);
            MessageHistoryList.Clear();
            foreach (var user_discussion in user_discussionAuthenticatedUserList)
            {                
                if (MessageHistoryList.Values.Where(x=>x.Message.DiscussionId == user_discussion.DiscussionId).Count() == 0)
                {
                    MessageModel lastMessage = new MessageModel();
                    var allMessages = await BL.BLMessage.searchMessage(new Message { DiscussionId = user_discussion.DiscussionId }, chatcommon.Enums.EOperator.AND);
                    var messageList = allMessages.Where(x => x.UserId != AuthenticatedUser.ID && x.Status == 1).OrderByDescending(x => x.Date).ToList();
                    if (messageList.Count > 0)
                    {
                        int nbCharToDisplay;
                        if(messageList.Where(x=>x.Status == 1).Count() > 0)
                            lastMessage = messageList.Where(x => x.Status == 1).Select(x => new MessageModel { Message = x }).First();
                        else
                            lastMessage = messageList.Select(x => new MessageModel { Message = x }).First();

                        lastMessage.TxtContent = Utility.decodeBase64ToString(lastMessage.TxtContent);
                        if (lastMessage.TxtContent.Length > 30)
                            nbCharToDisplay = 30;
                        else
                            nbCharToDisplay = lastMessage.TxtContent.Length;
                        lastMessage.TxtContent = lastMessage.TxtContent.Substring(0, nbCharToDisplay) + "...";
                    }
                    else if(allMessages.Count > 0)
                    {                        
                        var recipientsList = (await BL.BLUser_discussion.searchUser_discussion(new User_discussion { DiscussionId = user_discussion.DiscussionId }, chatcommon.Enums.EOperator.AND)).Where(x=>x.UserId != AuthenticatedUser.ID).ToList();
                        if(recipientsList.Count() > 0)
                           lastMessage.Message = new Message { Content = "...", DiscussionId = user_discussion.DiscussionId, UserId = recipientsList[0].UserId, Status = 0, Date = allMessages.OrderByDescending(x => x.Date).Select(x => x.Date).First() };    
                    }/**/                     
                               
                    var userList = await BL.BLUser.GetUserDataById(lastMessage.Message.UserId);
                    if (userList.Count > 0)
                        MessageHistoryList = concat(MessageHistoryList, new Dictionary<UserModel, MessageModel> { { userList.Select(x => new UserModel { User = x }).First(), lastMessage }});
                }  
            }         
        }

        private Dictionary<UserModel, MessageModel> concat(Dictionary<UserModel, MessageModel> dictTarget, Dictionary<UserModel, MessageModel> dictSource)
        {
            foreach (var dict in dictSource)
            {
                dictTarget.Add(dict.Key, dict.Value);
            }

            return dictTarget;
        }



    }
}
