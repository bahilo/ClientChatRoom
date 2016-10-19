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

namespace chatroom
{
    public class MainWindowViewModel : BindBase
    {
        private System.Net.Sockets.TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private Context _context;
        private Object _currentViewModel;
        
        public UserViewModel UserViewModel { get; set; }
        public DiscussionViewModel DiscussionViewModel { get; set; }
        public MessageViewModel MessageViewModel { get; set; }
        public SecurityLoginViewModel SecurityLoginViewModel { get; set; }

        public ButtonCommand<string> CommandNavig { get; set; }
        public ButtonCommand<string> LogOutCommand { get; set; }

        public MainWindowViewModel()
        {
            initializer();
            setInitEvents();
            setLogic();
            SecurityLoginViewModel.showView();
            //SecurityLoginViewModel.startAuthentication();
            //setEnvironment();


        }

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

        private void setInitEvents()
        {
            SecurityLoginViewModel.UserModel.PropertyChanged += onAuthenticatedAgentChange;
        }

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

        private void setLogic()
        {
            UserViewModel.Startup = _startup;
            DiscussionViewModel.Startup = _startup;
            MessageViewModel.Startup = _startup;
            SecurityLoginViewModel.Startup = _startup;      
        }

        /*private void setEnvironment()
        {
            DiscussionViewModel.ClientSocket = _clientSocket;            
            DiscussionViewModel.ServerStream = _serverStream;
        }*/

        private void initializer()
        {
            _startup = new Startup();
            _context = new Context(navigation);
            UserViewModel = new UserViewModel(navigation);
            DiscussionViewModel = new DiscussionViewModel(navigation);
            MessageViewModel = new MessageViewModel(navigation);
            SecurityLoginViewModel = new SecurityLoginViewModel(navigation);            
            CommandNavig = new ButtonCommand<string>(appNavig, canAppNavig);
            LogOutCommand = new ButtonCommand<string>(logOut, canLogOut);
            UserViewModel.Dialog = Dialog;
            DiscussionViewModel.Dialog = Dialog;
            MessageViewModel.Dialog = Dialog;
            SecurityLoginViewModel.Dialog = Dialog;


            //SecurityLoginViewModel.UserModel = UserViewModel.UserModel;
        }

        private void connectToServer()
        {       
            try
            {
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                string ipAddress = ConfigurationManager.AppSettings["IP"];
                _clientSocket = new System.Net.Sockets.TcpClient();
                _serverStream = default(NetworkStream);

                _clientSocket.Connect(ipAddress, port);
                DiscussionViewModel.ClientSocket = _clientSocket;
                DiscussionViewModel.ServerStream = _serverStream;
                /*_serverStream = _clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("0/"+_startup.Bl.BLSecurity.GetAuthenticatedUser().Username +"/0/"+ "$");//textBox3.Text
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();*/
                //DiscussionViewModel.msg("info", "Connected to Chat Server ...");
                Thread ctThread = new Thread(DiscussionViewModel.getMessage);
                ctThread.SetApartmentState(ApartmentState.STA);
                ctThread.Start();
            }
            catch (Exception ex)
            {
                DiscussionViewModel.msg("info", "Error while trying to connect to server!");
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
