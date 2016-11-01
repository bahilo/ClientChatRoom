using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatroom.Classes;
using chatroom.Commands;
using chatroom.Intefaces;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using chatcommon.Interfaces;

namespace chatroom.ViewModels
{
    public class DiscussionViewModel : BindBase, IDiscussionViewModel
    {
        private string _navigTo;

        private string _inputMessage;
        private string _outputMessage;
        private int _nbNewMessage;
        private System.Net.Sockets.TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private IChatRoom _chatRoom;
        private List<UserModel> _userDiscussionGroupList;
        private List<DiscussionModel> _discussionList;
        private Func<object, object> _page;
        private NotifyTaskCompletion<int> _discussionGroupCreationTask;
        private bool _isGroupDiscussion;
        private string _groupId;


        //----------------------------[ Models ]------------------

        private DiscussionModel _discussionModel;
        private UserModel _selectedUserModel;


        //----------------------------[ Commands ]------------------

        public ButtonCommand<object> SendMessageCommand { get; set; }
        public ButtonCommand<UserModel> SelectUserForDiscussionCommand { get; set; }
        public ButtonCommand<UserModel> SaveUserForDiscussionGroupCommand { get; set; }
        public ButtonCommand<object> DiscussionGroupCreationCommand { get; set; }
        public ButtonCommand<object> NavigToHomeCommand { get; set; }
        public ButtonCommand<object> ReadNewMessageCommand { get; set; }
        public ButtonCommand<UserModel> AddUserToDiscussionCommand { get; set; }
        public ButtonCommand<string> GetDiscussionGroupCommand { get; set; }



        public DiscussionViewModel()
        {
            instances();
            instancesModel();
            instancesCommand();
            initEvents();
        }

        public DiscussionViewModel(Func<object, object> navigation) : this()
        {
            this._page = navigation;
        }


        //----------------------------[ Initialization ]------------------


        private void initEvents()
        {
            PropertyChanged += onNavigToChange;
            PropertyChanged += onDiscussionModelChange;
            _discussionGroupCreationTask.PropertyChanged += onDiscussionGroupCreationTaskCompletion;
        }

        private void instances()
        {
            _clientSocket = new System.Net.Sockets.TcpClient();
            _serverStream = default(NetworkStream);
            _userDiscussionGroupList = new List<UserModel>();
            _discussionList = new List<DiscussionModel>();
            _discussionGroupCreationTask = new NotifyTaskCompletion<int>();            
        }


        private void instancesModel()
        {
            _selectedUserModel = new UserModel();
            _discussionModel = new DiscussionModel();
        }

        private void instancesCommand()
        {
            SendMessageCommand = new ButtonCommand<object>(sendMessage, canSendMessage);
            SelectUserForDiscussionCommand = new ButtonCommand<UserModel>(selectUserForDiscussion, canSelectUserForDiscussion);
            SaveUserForDiscussionGroupCommand = new ButtonCommand<UserModel>(saveUserForDiscussionGroup, canSelectUserForDiscussion);
            DiscussionGroupCreationCommand = new ButtonCommand<object>(createDiscussionGroup, canCreateDiscussionGroup);
            NavigToHomeCommand = new ButtonCommand<object>(goToHomePage, canGoToHomePage);
            ReadNewMessageCommand = new ButtonCommand<object>(readNewMessages, canReadNewMessages);
            AddUserToDiscussionCommand = new ButtonCommand<UserModel>(addUserToCurrentDiscussion, canAddUserToCurrentDiscussion);
            GetDiscussionGroupCommand = new ButtonCommand<string>(getDiscussionGroup, canGetDiscussionGroup);
        }


        //----------------------------[ Properties ]------------------


        public User AuthenticatedUser
        {
            get { return BL.BLSecurity.GetAuthenticatedUser(); }
        }

        public string NavigTo
        {
            get { return _navigTo; }
            set { setPropertyChange(ref _navigTo, value); }
        }

        public string TxtNbNewMessage
        {
            get { return _nbNewMessage.ToString(); }
            set { int converted; if (int.TryParse(value, out converted)) { setPropertyChange(ref _nbNewMessage, converted); } }
        }

        public IBusinessLogic BL
        {
            get { return _startup.BL; }
            set { _startup.BL = value; onPropertyChange("BL"); }
        }

        public DiscussionModel DiscussionModel
        {
            get { return _discussionModel; }
            set { setPropertyChange(ref _discussionModel, value); }
        }

        public IChatRoom ChatRoom
        {
            get { return _chatRoom; }
            set { setPropertyChange(ref _chatRoom, value); }
        }

        public UserModel SelectedUserModel
        {
            get { return _selectedUserModel; }
            set { setPropertyChange(ref _selectedUserModel, value); }
        }

        public bool IsGroupDiscussion
        {
            get { return _isGroupDiscussion; }
            set { setPropertyChange(ref _isGroupDiscussion, value); }
        }

        public List<DiscussionModel> DiscussionList
        {
            get { return _discussionList; }
            set { setPropertyChange(ref _discussionList, value); }
        }

        public string InputMessage
        {
            get { return _inputMessage; }
            set { setPropertyChange(ref _inputMessage, value); }
        }

        public string OutputMessage
        {
            get { return _outputMessage; }
            set { setPropertyChange(ref _outputMessage, value); }
        }

        public System.Net.Sockets.TcpClient ClientSocket
        {
            get { return _clientSocket; }
            set { setPropertyChange(ref _clientSocket, value); }
        }

        public NetworkStream ServerStream
        {
            get { return _serverStream; }
            set { setPropertyChange(ref _serverStream, value); }
        }


        //----------------------------[ Actions ]------------------


        public async void load()
        {
            Dialog.showSearch("Loading...");
            // get all discussions where the authenticated user appears
            DiscussionList = await retrieveUserDiscussions(AuthenticatedUser);
            
            // find the discussion where the selected user appears
            List<DiscussionModel> discussionFoundList = new List<DiscussionModel>();
            if(IsGroupDiscussion)
                discussionFoundList = DiscussionList.Where(x => x.TxtID == _groupId.Split('-')[2]).ToList();
            else
                discussionFoundList = DiscussionList.Where(x => x.UserList.Where(y => y.User.ID == SelectedUserModel.User.ID).Count() > 0 && x.UserList.Count == 1).ToList();

            // display discussion messages
            if (discussionFoundList.Count > 0)
            {
                DiscussionModel = discussionFoundList[0];
                var messageListToUpdate = (await BL.BLMessage.searchMessage(new Message { DiscussionId = discussionFoundList[0].Discussion.ID, Status = 1 }, chatcommon.Enums.EOperator.AND)).Where(x => x.UserId != AuthenticatedUser.ID).Select(x => new Message
                {
                    ID = x.ID,
                    Content = x.Content,
                    Date = x.Date,
                    DiscussionId = x.DiscussionId,
                    Status = 0,
                    UserId = x.UserId
                }).ToList();

                var updatedMessageList = await BL.BLMessage.UpdateMessage(messageListToUpdate);

                DiscussionModel.addUser(new UserModel { User = AuthenticatedUser });
                var messageList = (await BL.BLMessage.searchMessage(new Message { DiscussionId = DiscussionModel.Discussion.ID }, chatcommon.Enums.EOperator.AND)).OrderByDescending(x => x.ID).Take(5).ToList();
                foreach (var message in messageList.OrderBy(x => x.ID).ToList())
                    displayMessage(message, DiscussionModel.UserList.Where(x => x.User.ID == message.UserId).Select(x => x.User).FirstOrDefault());
            }
            Dialog.IsDialogOpen = false;
        }

        public async Task<List<DiscussionModel>> retrieveUserDiscussions(User user)
        {
            object _lock = new object();
            List<DiscussionModel> output = new List<DiscussionModel>();
            List<User_discussion> allUser_discussionOfAuthencatedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = user.ID }, chatcommon.Enums.EOperator.AND);
            foreach (User_discussion user_discussionOfAuthenticatedUser in allUser_discussionOfAuthencatedUserList)
            {
                if (output.Where(x => x.Discussion.ID == user_discussionOfAuthenticatedUser.DiscussionId).Count() == 0)
                {
                    // Get All users appearing in the same discusion as the authenticated user 
                    List<User_discussion> allUser_discussionOfOtherUserList = (await BL.BLUser_discussion.searchUser_discussion(new User_discussion { DiscussionId = user_discussionOfAuthenticatedUser.DiscussionId }, chatcommon.Enums.EOperator.AND)).Where(x => x.UserId != user.ID).ToList();

                    List<Discussion> discussionList = await BL.BLDiscussion.GetDiscussionDataById(user_discussionOfAuthenticatedUser.DiscussionId);
                    if (discussionList.Count > 0)
                    {
                        DiscussionModel discussion = new DiscussionModel();
                        discussion.Discussion = discussionList[0];

                        // Save all discussions and their users
                        foreach (User_discussion user_discussionOfOthers in allUser_discussionOfOtherUserList)
                        {
                            if (discussion.UserList.Where(x => x.User.ID == user_discussionOfOthers.UserId).Count() == 0)
                            {
                                List<User> userFoundList = await BL.BLUser.GetUserDataById(user_discussionOfOthers.UserId);
                                if (discussionList.Count > 0 && userFoundList.Count > 0)
                                    discussion.addUser(new UserModel { User = userFoundList[0] });
                            }
                        }
                        discussion.TxtGroupName = generateDiscussionGroupName(discussion.Discussion.ID, discussion.UserList);
                        lock(_lock)
                            output.Add(discussion);
                    }
                }
            }
            return output;
        }

        public void executeNavig(string obj)
        {
            switch (obj)
            {
                case "chatroom":
                    _page(this);
                    break;
                case "home":
                    _page(new MessageViewModel());
                    break;
                default:
                    goto case "home";
            }
        }

        public async void getMessage()
        {
            try
            {
                while (true)
                {
                    int discussionId = 0;
                    int userId = 0;
                    int messageId = 0;
                    List<string> composer = new List<string>();
                    string returndata = "";

                    _serverStream = _clientSocket.GetStream();
                    int buffSize = 0;
                    buffSize = _clientSocket.ReceiveBufferSize;
                    byte[] inStream = new byte[buffSize];
                    _serverStream.Read(inStream, 0, buffSize);
                    returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    returndata = returndata.Substring(0, returndata.IndexOf("$"));
                    composer = returndata.Split('/').ToList();

                    if (int.TryParse(composer[0], out discussionId) 
                        && int.TryParse(composer[1], out userId) 
                            && int.TryParse(composer[2], out messageId)
                                && discussionId > 0)
                    {
                        var messageFoundList = await BL.BLMessage.GetMessageDataById(messageId);

                        if (discussionId == DiscussionModel.Discussion.ID)
                        {
                            // if new group discussion detected reload discussion
                            if (composer[3].Split('|').Count() > DiscussionModel.UserList.Count)
                            {
                                IsGroupDiscussion = true;
                                _groupId = generateDiscussionGroupName(DiscussionModel.Discussion.ID, DiscussionModel.UserList);
                                NavigTo = "chatroom";
                            }
                            else
                            {
                                // current discussion messages displaying
                                var userFoundList = await BL.BLUser.GetUserDataById(userId);
                                if (messageFoundList.Count > 0 && userFoundList.Count > 0)
                                    displayMessage(messageFoundList[0], userFoundList[0]);
                            }                            
                        }
                        else
                        {
                            // notification of a new incoming message
                            TxtNbNewMessage = (_nbNewMessage + 1).ToString();
                            messageFoundList[0].Status = 1;
                            var updatedMessageList = await BL.BLMessage.UpdateMessage(new List<Message> { messageFoundList[0] });
                            System.Media.SystemSounds.Asterisk.Play();
                        }        
                    }
                    else
                    {
                        int recipientId;
                        var discussionList = await retrieveUserDiscussions(AuthenticatedUser);
                        var userIdList = composer[3].Split('|');
                        int.TryParse(userIdList[0], out recipientId);
                        
                        // update users status
                        //if(recipientId == AuthenticatedUser.ID)
                        //    onPropertyChange("updateStatus");
                        //else
                            //if (discussionList.Where(x=>x.UserList.Where(y=>y.User.ID == recipientId).Count() > 0).Count() > 0)
                                onPropertyChange("updateStatus");
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
        }

        public void displayMessage(Message message, User user)
        {
            if (user != null)
            {
                if (AuthenticatedUser.ID == message.UserId)
                {
                    message.Content = message.Date + Environment.NewLine + Utility.decodeBase64ToString(message.Content);
                    msg("reply", message.Content);
                }
                else
                {
                    message.Content = user.Username + " Says:" + Environment.NewLine + message.Date + Environment.NewLine + Utility.decodeBase64ToString(message.Content);
                    msg("recipient", message.Content);
                }
            }
        }

        public void msg(string type, string message)
        {
            if (ChatRoom != null)
                switch (type)
                {
                    case "info":
                        ChatRoom.showInfo(message);
                        break;
                    case "reply":
                        ChatRoom.showMyReply(Environment.NewLine + " >> " + message);
                        break;
                    case "recipient":
                        ChatRoom.showRecipientReply(Environment.NewLine + " >> " + message);
                        break;
                }
        }

        private async void validateDiscussionGroup()
        {
            if (_userDiscussionGroupList.Count > 0)
            {
                var discussionCreatedList = await BL.BLDiscussion.InsertDiscussion(new List<Discussion> { new Discussion { Date = DateTime.Now } });
                if (discussionCreatedList.Count > 0)
                {
                    // creating the link between the user and the discussion
                    _userDiscussionGroupList.Add(new UserModel { User = AuthenticatedUser });
                    var user_discussionCreatedList = await BL.BLUser_discussion.InsertUser_discussion(_userDiscussionGroupList.Select(x => new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = x.User.ID }).ToList());

                    // setting the current dicussion to the new discussion
                    DiscussionModel = new DiscussionModel { Discussion = discussionCreatedList[0] };
                    DiscussionModel.addUser(_userDiscussionGroupList.Where(x => x.User.ID != AuthenticatedUser.ID).ToList());
                    _selectedUserModel = _userDiscussionGroupList[0];

                    // navigate to the discussion view
                    NavigTo = "chatroom";
                }
            }
        }

        public string generateDiscussionGroupName(int discussionId, List<UserModel> userList)
        {
            string ouput = "";            
            string userGroup = "";
            string userIds = "";
            foreach (UserModel userModel in userList)
            {
                userGroup += userModel.TxtUserName + ";";
                userIds += userModel.TxtID + "|";
            }
            ouput += userGroup + "-" + userIds + "-" + discussionId;
            
            return ouput;
        }

        public override void Dispose()
        {
            base.Dispose();
            PropertyChanged -= onDiscussionModelChange;
            PropertyChanged -= onNavigToChange;
            _discussionGroupCreationTask.PropertyChanged -= onDiscussionGroupCreationTaskCompletion;
        }

        //----------------------------[ Event Handler ]------------------

        private void onNavigToChange(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "NavigTo"))
            {
                executeNavig(NavigTo);
            }
        }

        private void onDiscussionGroupCreationTaskCompletion(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSuccessfullyCompleted"))
            {
                if (Dialog.Response == 1)
                    validateDiscussionGroup();
                else
                    NavigTo = "home";
            }
        }

        private void onDiscussionModelChange(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "DiscussionModel"))
            {
                AddUserToDiscussionCommand.RaiseCanExecuteActionChanged();
            }
        }


        //----------------------------[ Action Commands ]------------------

        private async void sendMessage(object obj)
        {
            if (!string.IsNullOrEmpty(InputMessage) && SelectedUserModel.User.ID != 0)
            {
                if (DiscussionModel.Discussion.ID == 0)
                {
                    var discussionCreatedList = await BL.BLDiscussion.InsertDiscussion(new List<Discussion> { new Discussion { Date = DateTime.Now } });
                    if (discussionCreatedList.Count > 0)
                    {
                        var user_discussionCreatedList = await BL.BLUser_discussion.InsertUser_discussion(new List<User_discussion> {
                            new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = SelectedUserModel.User.ID },
                            new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = AuthenticatedUser.ID }
                        });
                        var discussionList = await retrieveUserDiscussions(AuthenticatedUser);
                        DiscussionModel = discussionList.Where(x=>x.Discussion.ID == discussionCreatedList[0].ID).FirstOrDefault() ?? new DiscussionModel { Discussion = discussionCreatedList[0] };
                    }
                }
                try
                {
                    var savedMdessage = await BL.BLMessage.InsertMessage(new List<Message> { new Message { DiscussionId = DiscussionModel.Discussion.ID, Content = InputMessage, Date = DateTime.Now, UserId = AuthenticatedUser.ID } });
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(DiscussionModel.TxtID + "/" + AuthenticatedUser.ID + "/" + savedMdessage[0].ID + "/" + generateDiscussionGroupName(DiscussionModel.Discussion.ID, DiscussionModel.UserList).Split('-')[1]  + "/" + InputMessage + "$"); //DiscussionModel.TxtGroupName.Split(';')[1] 
                    _serverStream.Write(outStream, 0, outStream.Length);
                    _serverStream.Flush();
                    InputMessage = "";
                }
                catch (Exception ex)
                {
                    Log.error(ex.Message);
                    msg("info", "Error while trying to send the message!");
                }
            }
        }

        private bool canSendMessage(object arg)
        {
            return true;
        }

        private void selectUserForDiscussion(UserModel obj)
        {
            Dialog.IsLeftBarClosed = false;
            IsGroupDiscussion = false;
            _groupId = "";
            DiscussionModel = new DiscussionModel();
            SelectedUserModel = obj;            

            // navig to discussion page
            NavigTo = "chatroom";
        }

        private bool canSelectUserForDiscussion(UserModel arg)
        {
            return true;
        }

        public void saveUserForDiscussionGroup(UserModel param)
        {
            if (!_userDiscussionGroupList.Contains(param))
                _userDiscussionGroupList.Add(param);
            else
                _userDiscussionGroupList.Remove(param);
        }

        private bool canaveUserForDiscussionGroup(UserModel arg)
        {
            return true;
        }

        private void createDiscussionGroup(object obj)
        {
            _discussionGroupCreationTask.initializeNewTask(Dialog.show(new Views.ChatGroup()));
        }

        private bool canCreateDiscussionGroup(object arg)
        {
            return true;
        }

        private void goToHomePage(object obj)
        {
            NavigTo = "home";
        }

        private bool canGoToHomePage(object arg)
        {
            return true;
        }

        private void readNewMessages(object obj)
        {
            TxtNbNewMessage = 0.ToString();
            NavigTo = "home";
        }

        private bool canReadNewMessages(object arg)
        {
            return true;
        }

        private async void addUserToCurrentDiscussion(UserModel obj)
        {
            Dialog.IsLeftBarClosed = false;
            var user_discussionSavedList = await BL.BLUser_discussion.InsertUser_discussion(new List<User_discussion> { new User_discussion { DiscussionId = DiscussionModel.Discussion.ID, UserId = obj.User.ID } });
            IsGroupDiscussion = true;
            DiscussionModel.addUser(obj);
            _groupId = generateDiscussionGroupName(DiscussionModel.Discussion.ID, DiscussionModel.UserList);
            NavigTo = "chatroom";
        }

        private bool canAddUserToCurrentDiscussion(UserModel arg)
        {
            if (DiscussionModel.Discussion.ID == 0)
                return false;

            if (arg != null && DiscussionModel.UserList.Where(x => x.User.ID == arg.User.ID).Count() > 0)
                return false;

            return true;
        }

        private void getDiscussionGroup(string obj)
        {
            Dialog.IsLeftBarClosed = false;
            SelectedUserModel = new UserModel { TxtID = obj.Split('-')[1].Split('|')[0] };
            IsGroupDiscussion = true;
            _groupId = obj;
            NavigTo = "chatroom";          
        }

        private bool canGetDiscussionGroup(string arg)
        {
            return true;
        }




    }
}
