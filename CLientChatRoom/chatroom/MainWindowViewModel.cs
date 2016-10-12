using chatcommon.Classes;
using chatroom.Classes;
using chatroom.Intefaces;
using chatroom.ViewModels;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

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

        public MainWindowViewModel(IMainWindow window)
        {
            MainWindow = window;
            initializer();
            setLogic();
            setEnvironment();
            connectToServer();
        }

        private void setLogic()
        {
            UserViewModel.Startup = _startup;
            DiscussionViewModel.Startup = _startup;
            MessageViewModel.Startup = _startup;
        }

        private void setEnvironment()
        {
            DiscussionViewModel.ClientSocket = _clientSocket;            
            DiscussionViewModel.ServerStream = _serverStream;
        }

        private void initializer()
        {
            _startup = new Startup();
            UserViewModel = new UserViewModel();
            DiscussionViewModel = new DiscussionViewModel();
            MessageViewModel = new MessageViewModel();
            _clientSocket = new System.Net.Sockets.TcpClient();
            _serverStream = default(NetworkStream);

            DiscussionViewModel.MainWindowViewModel = this;
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
            DiscussionViewModel.ReadData = "Connected to Chat Server ...";
            DiscussionViewModel.msg();

            try
            {
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                string ipAddress = ConfigurationManager.AppSettings["IP"];

                _clientSocket.Connect(ipAddress, port);
                _serverStream = _clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Test" + "$");//textBox3.Text
                _serverStream.Write(outStream, 0, outStream.Length);
                _serverStream.Flush();

                Thread ctThread = new Thread(DiscussionViewModel.getMessage);
                ctThread.Start();
            }
            catch (Exception ex)
            {
                DiscussionViewModel.ReadData = "You are already connected!";
                DiscussionViewModel.msg();
                Log.error(ex.Message);
            }

        }
    }
}
