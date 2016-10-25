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

namespace chatroom.ViewModels
{
    public class DiscussionViewModel : BindBase
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

        public BusinessLogic BL
        {
            get { return _startup.BL; }
            set { _startup.BL = value; onPropertyChange("BL"); }
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
            // Get all discussion where the authenticated user appear
            List<User_discussion> allUser_discussionOfAuthencatedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = AuthenticatedUser.ID }, chatcommon.Enums.EOperator.AND);
            DiscussionList.Clear();
            foreach (User_discussion user_discussionOfAuthenticatedUser in allUser_discussionOfAuthencatedUserList)
            {                
                if (DiscussionList.Where(x => x.Discussion.ID == user_discussionOfAuthenticatedUser.DiscussionId).Count() == 0)
                {
                    // update messages status
                    var messageList = (await BL.BLMessage.searchMessage(new Message { DiscussionId = user_discussionOfAuthenticatedUser.DiscussionId, Status = 1 }, chatcommon.Enums.EOperator.AND)).Where(x => x.UserId != AuthenticatedUser.ID).Select(x => new Message
                    {
                        ID = x.ID,
                        Content = x.Content,
                        Date = x.Date,
                        DiscussionId = x.DiscussionId,
                        Status = 0,
                        UserId = x.UserId
                    }).ToList();
                    var updatedMessageList = await BL.BLMessage.UpdateMessage(messageList);

                    // Get All users appearing in the same discusion as the authenticated user 
                    List<User_discussion> allUser_discussionOfOtherUserList = (await BL.BLUser_discussion.searchUser_discussion(new User_discussion { DiscussionId = user_discussionOfAuthenticatedUser.DiscussionId }, chatcommon.Enums.EOperator.AND)).Where(x => x.UserId != AuthenticatedUser.ID).ToList();

                    List<Discussion> discussionList = await BL.BLDiscussion.GetDiscussionDataById(user_discussionOfAuthenticatedUser.DiscussionId);
                    DiscussionModel discussion = new DiscussionModel();
                    discussion.Discussion = discussionList[0];

                    // Save all discussions and their users
                    foreach (User_discussion user_discussionOfOthers in allUser_discussionOfOtherUserList)
                    {
                        if (discussion.UserList.Where(x=>x.User.ID == user_discussionOfOthers.UserId).Count() == 0)
                        {
                            List<User> userFoundList = await BL.BLUser.GetUserDataById(user_discussionOfOthers.UserId);
                            if (discussionList.Count > 0 && userFoundList.Count > 0)
                                discussion.addUser(new UserModel { User = userFoundList[0] });
                        }
                                                  
                    }
                    DiscussionList.Add(discussion);
                }
            }


            // Find the discussion where the selected user is appearing
            var discussionFoundList = DiscussionList.Where(x=>x.UserList.Where(y=>y.User.ID == SelectedUserModel.User.ID).Count() > 0).ToList();

            if (discussionFoundList.Count > 0)
            {
                _discussionModel = discussionFoundList[0];
                _discussionModel.addUser(new UserModel { User = AuthenticatedUser });
                var messageList = (await BL.BLMessage.searchMessage(new Message { DiscussionId = _discussionModel.Discussion.ID }, chatcommon.Enums.EOperator.AND)).OrderByDescending(x => x.ID).Take(5).ToList();
                foreach (var message in messageList.OrderBy(x => x.ID).ToList())
                    displayMessage(message, _discussionModel.UserList.Where(x => x.User.ID == message.UserId).Select(x => x.User).FirstOrDefault());
            }

            /*if (SelectedUserModel != null)
            {
                var user_discussionSelectedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = SelectedUserModel.User.ID }, chatcommon.Enums.EOperator.AND);

                if (user_discussionSelectedUserList.Count > 0)
                {
                    foreach (var user_discussion in user_discussionSelectedUserList)
                    {
                        // searching all common discussion
                        var user_discussionAuthenticatedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = AuthenticatedUser.ID, DiscussionId = user_discussion.DiscussionId }, chatcommon.Enums.EOperator.AND);

                        // if common discussion found display messages
                        if (user_discussionAuthenticatedUserList.Count > 0)
                        {
                            var discussionFoundList = await BL.BLDiscussion.GetDiscussionDataById(user_discussionAuthenticatedUserList[0].DiscussionId);
                            if (discussionFoundList.Count > 0)
                            {
                                _discussionModel = new DiscussionModel { Discussion = discussionFoundList[0] };
                                _discussionModel.UserList.Add(SelectedUserModel);
                                var messageList = (await BL.BLMessage.searchMessage(new Message { DiscussionId = _discussionModel.Discussion.ID }, chatcommon.Enums.EOperator.AND)).OrderByDescending(x => x.ID).Take(5).ToList();
                                foreach (var message in messageList.OrderBy(x => x.ID).ToList())//
                                {
                                    if (_discussionModel.UserList.Where(x => x.User.ID == message.UserId).Count() == 0)
                                    {
                                        var userFound = (await BL.BLUser.GetUserDataById(message.UserId)).FirstOrDefault();
                                        if (userFound != null)
                                            _discussionModel.UserList.Add(new UserModel { User = userFound });
                                    }
                                    displayMessage(message, _discussionModel.UserList.Where(x => x.User.ID == message.UserId).Select(x => x.User).FirstOrDefault());

                                }
                            }
                        }
                    }
                }
            } */
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

                    if (int.TryParse(composer[0], out discussionId) && int.TryParse(composer[1], out userId) && int.TryParse(composer[2], out messageId))
                    {
                        var messageFoundList = await BL.BLMessage.GetMessageDataById(messageId);

                        if (discussionId == _discussionModel.Discussion.ID)
                        {                            
                            var userFoundList = await BL.BLUser.GetUserDataById(userId);
                            if (messageFoundList.Count > 0 && userFoundList.Count > 0)
                                displayMessage(messageFoundList[0], userFoundList[0]);
                        }
                        else
                        {
                            TxtNbNewMessage = (_nbNewMessage + 1).ToString();
                            messageFoundList[0].Status = 1;
                            var updatedMessageList = await BL.BLMessage.UpdateMessage(new List<Message> { messageFoundList[0] });
                            System.Media.SystemSounds.Asterisk.Play();
                        }
                            
                    }
                    //msg("recipient", returndata.Substring(0, returndata.IndexOf("$")));
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
                    var user_discussionCreatedList = await BL.BLUser_discussion.InsertUser_discussion(_userDiscussionGroupList.Select(x => new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = x.User.ID }).ToList());
                }
            }
        }

        private string generateUserIdListString(List<UserModel> userModelList)
        {
            string output = "";
            foreach (UserModel userModel in userModelList)
            {
                output += userModel.TxtID+"|";
            }
            return output;
        }

        /*public static void broadcast(string msg)
        {
            try
            {
                var clientsByDiscussionList = clientsList.Where(x => x.Key.Split('/')[0] == msg.Split('/')[0]).Select(x => x.Value).ToList();
                foreach (TcpClient tcpClient in clientsByDiscussionList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = tcpClient;
                    if (broadcastSocket.Connected)
                    {
                        NetworkStream broadcastStream = broadcastSocket.GetStream();
                        Byte[] broadcastBytes = null;

                        broadcastBytes = Encoding.ASCII.GetBytes(msg + "$");

                        broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        broadcastStream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
        }  //end broadcast function
        */
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
            if (e.PropertyName.Equals("IsSuccessfullyCompleted") && Dialog.Response == 1)
            {
                validateDiscussionGroup();
            }
        }



        //----------------------------[ Action Commands ]------------------

        private async void sendMessage(object obj)
        {
            if (!string.IsNullOrEmpty(InputMessage))
            {
                if (_discussionModel.Discussion.ID == 0)
                {
                    var discussionCreatedList = await BL.BLDiscussion.InsertDiscussion(new List<Discussion> { new Discussion { Date = DateTime.Now } });
                    if (discussionCreatedList.Count > 0)
                    {
                        _discussionModel = new DiscussionModel { Discussion = discussionCreatedList[0] };
                        _discussionModel.UserList.Add(SelectedUserModel);
                        var user_discussionCreatedList = await BL.BLUser_discussion.InsertUser_discussion(new List<User_discussion> {
                            new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = SelectedUserModel.User.ID },
                            new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = AuthenticatedUser.ID }
                        });
                    }
                }

                if (!_discussionModel.UserList.Contains(SelectedUserModel))
                {
                    var user_discussionCreatedList = await BL.BLUser_discussion.InsertUser_discussion(new List<User_discussion> {
                            new User_discussion { DiscussionId = _discussionModel.Discussion.ID, UserId = SelectedUserModel.User.ID }
                        });
                }

                try
                {
                    var savedMdessage = await BL.BLMessage.InsertMessage(new List<Message> { new Message { DiscussionId = _discussionModel.Discussion.ID, Content = InputMessage, Date = DateTime.Now, UserId = AuthenticatedUser.ID } });
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(_discussionModel.TxtID + "/" + AuthenticatedUser.ID + "/" + savedMdessage[0].ID + "/" + generateUserIdListString(_discussionModel.UserList) + "/" + InputMessage + "$"); // 
                    _serverStream.Write(outStream, 0, outStream.Length);
                    _serverStream.Flush();
                    InputMessage = "";
                }
                catch (Exception ex)
                {
                    Log.error(ex.Message);
                    msg("info", "Error while trying to send message!");
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
            SelectedUserModel = obj;
            SelectUserForDiscussionCommand.RaiseCanExecuteActionChanged();
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

        private void addUserToCurrentDiscussion(UserModel obj)
        {
            _discussionModel.addUser(obj);
        }

        private bool canAddUserToCurrentDiscussion(UserModel arg)
        {
            if (_discussionModel.Discussion.ID == 0)
                return false;

            if (arg != null && _discussionModel.UserList.Where(x=>x.User.ID == arg.User.ID).Count() > 0)
                return false;

            return true;
        }

    }
}
