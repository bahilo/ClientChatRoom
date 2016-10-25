using chatcommon.Classes;
using chatroom.Classes;
using chatroom.Intefaces;
using chatroom.ViewModels;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using chatroom.Commands;
using chatroom.Interfaces;
using System.Windows;
using chatroom.Models;
using chatcommon.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace chatroom
{
    public class MainWindowViewModel : BindBase
    {
        private System.Net.Sockets.TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private Context _context;
        private Object _currentViewModel;
        private bool _isServerConnectionError;


        //----------------------------[ ViewModels ]------------------

        public UserViewModel UserViewModel { get; set; }
        public DiscussionViewModel DiscussionViewModel { get; set; }
        public MessageViewModel MessageViewModel { get; set; }
        public SecurityLoginViewModel SecurityLoginViewModel { get; set; }


        //----------------------------[ Commands ]------------------

        public ButtonCommand<string> CommandNavig { get; set; }
        public ButtonCommand<string> LogOutCommand { get; set; }
        


        public MainWindowViewModel()
        {
            initializer();
            setInitEvents();
            setLogic();
            SecurityLoginViewModel.showView();
        }


        //----------------------------[ Initialization ]------------------

            
        private void initializer()
        {
            _startup = new Startup();
            _context = new Context(navigation);
            DiscussionViewModel = new DiscussionViewModel(navigation);
            UserViewModel = new UserViewModel(navigation, DiscussionViewModel);
            MessageViewModel = new MessageViewModel(navigation, DiscussionViewModel);
            SecurityLoginViewModel = new SecurityLoginViewModel(navigation);
            CommandNavig = new ButtonCommand<string>(appNavig, canAppNavig);
            LogOutCommand = new ButtonCommand<string>(logOut, canLogOut);
            UserViewModel.Dialog = Dialog;
            DiscussionViewModel.Dialog = Dialog;
            MessageViewModel.Dialog = Dialog;
            SecurityLoginViewModel.Dialog = Dialog;
        }

        private void setLogic()
        {
            UserViewModel.Startup = _startup;
            DiscussionViewModel.Startup = _startup;
            MessageViewModel.Startup = _startup;
            SecurityLoginViewModel.Startup = _startup;
        }

        private void setInitEvents()
        {
            SecurityLoginViewModel.UserModel.PropertyChanged += onAuthenticatedAgentChange;
            DiscussionViewModel.PropertyChanged += onChatRoomChange;
        }


        //----------------------------[ Properties ]------------------


        public Object CurrentViewModel
        {
            get { return _currentViewModel; }
            set { setPropertyChange(ref _currentViewModel, value); }
        }

        public string TxtUserName
        {
            get { return (_startup.BL != null) ? _startup.BL.BLSecurity.GetAuthenticatedUser().Username : ""; }
        }

        public Context Context
        {
            get { return _context; }
            set { setPropertyChange(ref _context, value); }
        }


        //----------------------------[ Actions ]------------------

        private async void connectToServer()
        {
            try
            {
                // initialize the communication with the server
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                string ipAddress = ConfigurationManager.AppSettings["IP"];
                _clientSocket = new System.Net.Sockets.TcpClient();
                _serverStream = default(NetworkStream);

                // sign in the authenticated user on the server
                _clientSocket.Connect(ipAddress, port);
                DiscussionViewModel.ClientSocket = _clientSocket;
                DiscussionViewModel.ServerStream = _serverStream;
                _serverStream = _clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("0/" + _startup.BL.BLSecurity.GetAuthenticatedUser().ID + "/0/" + "$");//textBox3.Text
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();

                // update user status (1 = connected / 0 = disconnected)
                User authenticatedUser = _startup.BL.BLSecurity.GetAuthenticatedUser();
                authenticatedUser.Status = 1;
                var updatedUserList = await _startup.BL.BLUser.UpdateUser(new List<User> { authenticatedUser });
                
                // create discussion thread
                Thread ctThread = new Thread(DiscussionViewModel.getMessage);
                ctThread.SetApartmentState(ApartmentState.STA);
                ctThread.Start();
            }
            catch (Exception ex)
            {
                _isServerConnectionError = true;
                CurrentViewModel = DiscussionViewModel;
                Log.error(ex.Message);
            }
        }

        public object navigation(object centralPageContent = null)
        {
            if (centralPageContent != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Context.PreviousState = CurrentViewModel as IState;
                    CurrentViewModel = centralPageContent;
                    Context.NextState = centralPageContent as IState;
                });
            }
            return CurrentViewModel;
        }

        private void cleanUp()
        {
            if (_clientSocket != null && _clientSocket.Connected)
            {
                signOutFromServer();
                _clientSocket.GetStream().Close();
                _clientSocket.Close();
                _serverStream.Close();
            }

            // unsubscribe events
            SecurityLoginViewModel.UserModel.PropertyChanged -= onAuthenticatedAgentChange;
            DiscussionViewModel.PropertyChanged -= onChatRoomChange;
        }

        private async void signOutFromServer()
        {
            if (_serverStream != null)
            {
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("-1/" + _startup.BL.BLSecurity.GetAuthenticatedUser().ID + "/0/" + "$");//textBox3.Text
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();/**/

                // update user status to disconnected
                User authenticatedUser = _startup.BL.BLSecurity.GetAuthenticatedUser();
                authenticatedUser.Status = 0;
                var updatedUserList = await _startup.BL.BLUser.UpdateUser(new List<User> { authenticatedUser });
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            UserViewModel.Dispose();
            DiscussionViewModel.Dispose();
            MessageViewModel.Dispose();
            SecurityLoginViewModel.Dispose();
            cleanUp();
        }

        //----------------------------[ Event Handler ]------------------


        private async void onAuthenticatedAgentChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("User"))
            {
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    onPropertyChange("TxtUserName");
                    connectToServer();
                    UserViewModel.load();
                }));
            }
        }

        private void onChatRoomChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ChatRoom") && _isServerConnectionError)
            {
                _isServerConnectionError = false;
                DiscussionViewModel.msg("info", "Error while trying to connect to server!");
            }
        }


        //----------------------------[ Action Commands ]------------------
        

        private void appNavig(string obj)
        {
            switch (obj)
            {
                case "chat":
                    CurrentViewModel = DiscussionViewModel;
                    break;
                case "back":
                    Context.Request();
                    break;
                default:
                    CurrentViewModel = MessageViewModel;
                    break;
            }
        }

        private bool canAppNavig(string arg)
        {
            return true;
        }

        private void logOut(string obj)
        {
            SecurityLoginViewModel.showView();
        }

        private bool canLogOut(string arg)
        {
            return true;
        }
    }
}
