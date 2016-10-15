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

namespace chatroom.ViewModels
{
    public class DiscussionViewModel : BindBase
    {
        private string _inputMessage;
        private string _outputMessage;
        private System.Net.Sockets.TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private IMainWindowViewModel _main;
        private UserModel _selectedUserModel;
        //private List

        public ButtonCommand<object> SendMessageCommand { get; set; }
        public ButtonCommand<UserModel> SelectUserForDiscussionCommand { get; set; }

        public DiscussionViewModel()
        {
            _clientSocket = new System.Net.Sockets.TcpClient();
            _serverStream = default(NetworkStream);
            _selectedUserModel = new UserModel();

            SendMessageCommand = new ButtonCommand<object>(sendMessage, canSendMessage);
            SelectUserForDiscussionCommand = new ButtonCommand<UserModel>(selectUserForDiscussion, canSelectUserForDiscussion);
        }

        public BusinessLogic BL
        {
            get { return _startup.Bl; }
            set { _startup.Bl = value; onPropertyChange("BL"); }
        }

        public UserModel SelectedUserModel
        {
            get { return _selectedUserModel; }
            set { setPropertyChange(ref _selectedUserModel, value); }
        }

        public IMainWindowViewModel MainWindowViewModel
        {
            get { return _main; }
            set { _main = value; onPropertyChange("MainWindowViewModel"); }
        }

        public string InputMessage
        {
            get
            { return _inputMessage; }
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


        public void getMessage()
        {
            try
            {
                while (true)
                {
                    _serverStream = _clientSocket.GetStream();
                    int buffSize = 0;
                    buffSize = _clientSocket.ReceiveBufferSize;
                    byte[] inStream = new byte[buffSize];
                    _serverStream.Read(inStream, 0, buffSize);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    msg("recipient", returndata.Substring(0, returndata.IndexOf("$")));
                }
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
        }

        public void msg(string type, string message)
        {
            IMainWindow window = MainWindowViewModel.getObject("window") as IMainWindow;
            if (window != null)
            {
                switch (type)
                {
                    case "info":
                        window.showInfo(message);
                        break;
                    case "reply":
                        window.showMyReply(Environment.NewLine + " >> " + message);
                        break;
                    case "recipient":
                        window.showRecipientReply(Environment.NewLine + " >> " + message);
                        break;
                }
            }
        }

        private void sendMessage(object obj)
        {
            if (!string.IsNullOrEmpty(InputMessage))
            {
                //var savedMdessage = await BL.BLMessage.InsertMessage();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(InputMessage + "$");
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();
            }
            
            //getMessage();
        }

        private bool canSendMessage(object arg)
        {
            return true;
        }

        private async void selectUserForDiscussion(UserModel obj)
        {
            Dialog.IsLeftBarClosed = false;
            SelectedUserModel = obj;
            User authenticatedUser = BL.BLSecurity.GetAuthenticatedUser();
            var user_discussionSelectedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = SelectedUserModel.User.ID }, chatcommon.Enums.EOperator.AND);

            if (user_discussionSelectedUserList.Count > 0)
            {
                // searching all common discussion
                var user_discussionAuthenticatedUserList = await BL.BLUser_discussion.searchUser_discussion(new User_discussion { UserId = authenticatedUser.ID, DiscussionId = user_discussionSelectedUserList[0].DiscussionId }, chatcommon.Enums.EOperator.AND);

                // if common discussion found display messages
                if (user_discussionAuthenticatedUserList.Count > 0)
                {
                    var messageList = (await BL.BLMessage.searchMessage(new Message { DiscussionId = user_discussionAuthenticatedUserList[0].DiscussionId }, chatcommon.Enums.EOperator.AND)).OrderBy(x => x.Date).ToList();
                    foreach (var message in messageList)
                    {
                        if (obj.User.ID == message.UserId)
                            msg("recipient", message.Content);
                        if (authenticatedUser.ID == message.UserId)
                            msg("reply", message.Content);
                    }
                }
            }
            else
            {
                var discussionCreatedList = await BL.BLDiscussion.InsertDiscussion(new List<Discussion> { new Discussion { Date = DateTime.Now } });
                if (discussionCreatedList.Count > 0)
                {
                    var user_discussionCreatedList = await BL.BLUser_discussion.InsertUser_discussion(new List<User_discussion> {
                            new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = SelectedUserModel.User.ID },
                            new User_discussion { DiscussionId = discussionCreatedList[0].ID, UserId = authenticatedUser.ID }
                        });
                }
            }
        }

        private bool canSelectUserForDiscussion(UserModel arg)
        {
            return true;
        }

    }
}
