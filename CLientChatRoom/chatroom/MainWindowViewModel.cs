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

namespace chatroom
{
    public class MainWindowViewModel : BindBase, IMainWindowViewModel
    {
        private System.Net.Sockets.TcpClient _clientSocket;
        private NetworkStream _serverStream;

        public IMainWindow MainWindow { get; set; }
        public UserViewModel UserViewModel { get; set; }
        public DiscussionViewModel DiscussionViewModel { get; set; }
        public MessageViewModel MessageViewModel { get; set; }
        public SecurityLoginViewModel SecurityLoginViewModel { get; set; }

        public MainWindowViewModel(IMainWindow window) : base()
        {
            MainWindow = window;
            initializer();
            setInitEvents();
            setLogic();
            SecurityLoginViewModel.showView();
            //SecurityLoginViewModel.startAuthentication();
            setEnvironment();
            connectToServer();

        }

        private void setInitEvents()
        {
            SecurityLoginViewModel.UserModel.PropertyChanged += onAuthenticatedAgentChange;
        }

        private async void onAuthenticatedAgentChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("User"))
            {
                await MainWindow.onUIThreadAsync(() =>
                {
                    UserViewModel.load();
                });

            }
        }

        private void setLogic()
        {
            UserViewModel.Startup = _startup;
            DiscussionViewModel.Startup = _startup;
            MessageViewModel.Startup = _startup;
            SecurityLoginViewModel.Startup = _startup;      
        }

        private void setEnvironment()
        {
            DiscussionViewModel.ClientSocket = _clientSocket;            
            DiscussionViewModel.ServerStream = _serverStream;
        }

        private void initializer()
        {
            //Dialog.showSearch("Loading...");
            _startup = new Startup();
            //Dialog.IsDialogOpen = false;
            UserViewModel = new UserViewModel();
            DiscussionViewModel = new DiscussionViewModel();
            MessageViewModel = new MessageViewModel();
            SecurityLoginViewModel = new SecurityLoginViewModel();
            _clientSocket = new System.Net.Sockets.TcpClient();
            _serverStream = default(NetworkStream);

            DiscussionViewModel.MainWindowViewModel = this;
            UserViewModel.Dialog = Dialog;
            DiscussionViewModel.Dialog = Dialog;
            MessageViewModel.Dialog = Dialog;
            SecurityLoginViewModel.Dialog = Dialog;
            //SecurityLoginViewModel.UserModel = UserViewModel.UserModel;
        }

        public object getObject(string objectName)
        {
            object ObjectToReturn = new object();
            switch (objectName.ToUpper())
            {                
                case "WINDOW":
                    ObjectToReturn = MainWindow;
                    break;
            }

            return ObjectToReturn;
        }

        private void connectToServer()
        {       
            try
            {
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                string ipAddress = ConfigurationManager.AppSettings["IP"];

                _clientSocket.Connect(ipAddress, port);
                _serverStream = _clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Test" + "$");//textBox3.Text
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();
                DiscussionViewModel.msg("info", "Connected to Chat Server ...");
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
    }
}
