using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatcommon.Interfaces;
using chatroom.Classes;
using chatroom.Intefaces;
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
        private Dictionary<UserModel, MessageModel> _messageIndividualHistoryList;
        private Dictionary<UserModel, MessageModel> _messageGroupHistoryList;
        private IDiscussionViewModel _discussionViewModel;

        public MessageViewModel()
        {
            _messageIndividualHistoryList = new Dictionary<UserModel, MessageModel>();
            _messageGroupHistoryList = new Dictionary<UserModel, MessageModel>();
        }

        public MessageViewModel(Func<object, object> navigation, IDiscussionViewModel discussionViewModel) : this()
        {
            this.navigation = navigation;
            _discussionViewModel = discussionViewModel;
        }

        public User AuthenticatedUser
        {
            get { return BL.BLSecurity.GetAuthenticatedUser(); }
        }

        public IBusinessLogic BL
        {
            get { return _startup.BL; }
            set { _startup.BL = value; onPropertyChange("BL"); }
        }

        public Dictionary<UserModel, MessageModel> MessageIndividualHistoryList
        {
            get { return _messageIndividualHistoryList; }
            set { setPropertyChange(ref _messageIndividualHistoryList, value); }
        }

        public Dictionary<UserModel, MessageModel> MessageGroupHistoryList
        {
            get { return _messageGroupHistoryList; }
            set { setPropertyChange(ref _messageGroupHistoryList, value); }
        }

        public async void load()
        {
            Dialog.showSearch("Loading...");       
            MessageIndividualHistoryList.Clear();
            MessageGroupHistoryList.Clear();

            // searching all common discussion
            var discussionList = await _discussionViewModel.retrieveUserDiscussions(BL.BLSecurity.GetAuthenticatedUser());

            foreach (var discussionModel in discussionList)
            {                
                if (MessageIndividualHistoryList.Values.Where(x=>x.Message.DiscussionId == discussionModel.Discussion.ID).Count() == 0
                    && MessageGroupHistoryList.Values.Where(x => x.Message.DiscussionId == discussionModel.Discussion.ID).Count() == 0)
                {
                    MessageModel lastMessage = new MessageModel();
                    var allMessages = await BL.BLMessage.searchMessage(new Message { DiscussionId = discussionModel.Discussion.ID }, chatcommon.Enums.EOperator.AND);
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
                        if(discussionModel.UserList.Count() > 0)
                           lastMessage.Message = new Message { Content = "...", DiscussionId = discussionModel.Discussion.ID, UserId = discussionModel.UserList[0].User.ID, Status = 0, Date = allMessages.OrderByDescending(x => x.Date).Select(x => x.Date).First() };    
                    }/**/

                    lastMessage.TxtGroupName = discussionModel.TxtGroupName;          
                    var userList = await BL.BLUser.GetUserDataById(lastMessage.Message.UserId);
                    if (userList.Count > 0)
                    {
                        if(discussionModel.UserList.Count == 1)
                            MessageIndividualHistoryList = Utility.concat(MessageIndividualHistoryList, new Dictionary<UserModel, MessageModel> { { userList.Select(x => new UserModel { User = x }).First(), lastMessage } });
                        else
                            MessageGroupHistoryList = Utility.concat(MessageGroupHistoryList, new Dictionary<UserModel, MessageModel> { { userList.Select(x => new UserModel { User = x }).First(), lastMessage } });
                    }                        
                }  
            }
            Dialog.IsDialogOpen = false;         
        }

        



    }
}
