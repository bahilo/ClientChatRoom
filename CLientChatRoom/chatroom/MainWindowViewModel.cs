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
        private NotifyTaskCompletion<List<DiscussionModel>> _retrieveUserDiscussionTask_exitApp;
        private NotifyTaskCompletion<List<DiscussionModel>> _retrieveUserDiscussionTask_logOut;


        //----------------------------[ ViewModels ]------------------

        public UserViewModel UserViewModel { get; set; }
        public DiscussionViewModel DiscussionViewModel { get; set; }
        public MessageViewModel MessageViewModel { get; set; }
        public SecurityLoginViewModel SecurityLoginViewModel { get; set; }


        //----------------------------[ Commands ]------------------

        public ButtonCommand<string> CommandNavig { get; set; }
        public ButtonCommand<string> LogOutCommand { get; set; }
        public ButtonCommand<object> ExitCommand { get; set; }



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

            _retrieveUserDiscussionTask_exitApp = new NotifyTaskCompletion<List<DiscussionModel>>();
            _retrieveUserDiscussionTask_logOut = new NotifyTaskCompletion<List<DiscussionModel>>();

            DiscussionViewModel = new DiscussionViewModel(navigation);
            UserViewModel = new UserViewModel(navigation, DiscussionViewModel);
            MessageViewModel = new MessageViewModel(navigation, DiscussionViewModel);
            SecurityLoginViewModel = new SecurityLoginViewModel(navigation);

            CommandNavig = new ButtonCommand<string>(appNavig, canAppNavig);
            LogOutCommand = new ButtonCommand<string>(logOut, canLogOut);
            ExitCommand = new ButtonCommand<object>(exitApp, canExitApp);

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
            DiscussionViewModel.PropertyChanged += onUpdateStatusChange;
            _retrieveUserDiscussionTask_exitApp.PropertyChanged += onRetrieveUserDiscussionTaskCompletion_exitApp;
            _retrieveUserDiscussionTask_logOut.PropertyChanged += onRetrieveUserDiscussionTaskCompletion_logOut;
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
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("-2/" + _startup.BL.BLSecurity.GetAuthenticatedUser().ID + "/0/" + _startup.BL.BLSecurity.GetAuthenticatedUser().ID + "|" + "$");//textBox3.Text
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
                
                _clientSocket.GetStream().Close();
                _clientSocket.Close();
                _serverStream.Close();
            }            
        }

        private void unSubscribeEvents()
        {
            // unsubscribe events
            SecurityLoginViewModel.UserModel.PropertyChanged -= onAuthenticatedAgentChange;
            DiscussionViewModel.PropertyChanged -= onChatRoomChange;
            DiscussionViewModel.PropertyChanged -= onUpdateStatusChange;
            _retrieveUserDiscussionTask_exitApp.PropertyChanged -= onRetrieveUserDiscussionTaskCompletion_exitApp;
        }

        private async void signOutFromServer(List<DiscussionModel> discussionList)
        {
            try
            {
                if (_serverStream != null && discussionList.Count > 0)
                {
                    List<UserModel> userList = new List<UserModel>();
                    discussionList.Select(x => Utility.concat(userList, x.UserList)).First();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes("-1/" + _startup.BL.BLSecurity.GetAuthenticatedUser().ID + "/0/" + DiscussionViewModel.generateDiscussionGroupName(discussionList[0].Discussion.ID, userList).Split('-')[1] + "$");//textBox3.Text
                    _serverStream.Write(outStream, 0, outStream.Length);
                    _serverStream.Flush();
                }
            }
            finally
            {
                // update user status to disconnected
                User authenticatedUser = _startup.BL.BLSecurity.GetAuthenticatedUser();
                authenticatedUser.Status = 0;
                await _startup.BL.BLUser.UpdateUser(new List<User> { authenticatedUser });
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
            unSubscribeEvents();
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
                    //UserViewModel.load();
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

        private async void onUpdateStatusChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("updateStatus"))
            {
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    UserViewModel.load();
                }));
            }
        }

        private void onRetrieveUserDiscussionTaskCompletion_exitApp(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSuccessfullyCompleted"))
            {
                signOutFromServer(_retrieveUserDiscussionTask_exitApp.Result);
                Dispose();
                Dialog.IsDialogOpen = false;
                Application.Current.Shutdown();
            }
        }

        private void onRetrieveUserDiscussionTaskCompletion_logOut(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSuccessfullyCompleted"))
            {
                signOutFromServer(_retrieveUserDiscussionTask_logOut.Result);
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
            _retrieveUserDiscussionTask_logOut.initializeNewTask(DiscussionViewModel.retrieveUserDiscussions(_startup.BL.BLSecurity.GetAuthenticatedUser()));
            SecurityLoginViewModel.showView();
        }

        private bool canLogOut(string arg)
        {
            return true;
        }

        private void exitApp(object obj)
        {
            Dialog.showSearch("Closing...");
            _retrieveUserDiscussionTask_exitApp.initializeNewTask(DiscussionViewModel.retrieveUserDiscussions(_startup.BL.BLSecurity.GetAuthenticatedUser()));
         }

        private bool canExitApp(object arg)
        {
            return true;
        }
    }
}
