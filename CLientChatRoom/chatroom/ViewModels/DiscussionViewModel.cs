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
        private System.Net.Sockets.TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private IChatRoom _chatRoom;
        private Func<object, object> _page;


        //----------------------------[ Models ]------------------

        private DiscussionModel _discussionModel;
        private UserModel _selectedUserModel;


        //----------------------------[ Commands ]------------------

        public ButtonCommand<object> SendMessageCommand { get; set; }
        public ButtonCommand<UserModel> SelectUserForDiscussionCommand { get; set; }



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
        }

        private void instances()
        {
            _clientSocket = new System.Net.Sockets.TcpClient();
            _serverStream = default(NetworkStream);
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
            if (SelectedUserModel != null)
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
            }            
        }

        public void executeNavig(string obj)
        {
            switch (obj)
            {
                case "chatroom":
                    _page(this);
                    break;
                default:
                    goto case "chatroom";
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
                        var userFoundList = await BL.BLUser.GetUserDataById(userId);
                        if (messageFoundList.Count > 0 && userFoundList.Count > 0)
                            displayMessage(messageFoundList[0], userFoundList[0]);
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

        //----------------------------[ Event Handler ]------------------

        private void onNavigToChange(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "NavigTo"))
            {
                executeNavig(NavigTo);
            }
        }



        //----------------------------[ Action Commands ]------------------

        private async void sendMessage(object obj)
        {
            if (!string.IsNullOrEmpty(InputMessage))
            {
                if(_discussionModel.Discussion.ID == 0)
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
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(_discussionModel.TxtID + "/" + AuthenticatedUser.ID + "/" + savedMdessage[0].ID + "/" + InputMessage + "$"); // 
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

            // navig to discussion page
            NavigTo = "chatroom";
        }

        private bool canSelectUserForDiscussion(UserModel arg)
        {
            return true;
        }

    }
}
