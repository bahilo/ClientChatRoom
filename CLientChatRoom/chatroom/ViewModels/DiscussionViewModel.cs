using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatroom.Classes;
using chatroom.Commands;
using chatroom.Intefaces;
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
        private string _readData;
        private IMainWindowViewModel _main;

        public ButtonCommand<object> SendMessageCommand { get; set; }

        public DiscussionViewModel()
        {
            _clientSocket = new System.Net.Sockets.TcpClient();
            _serverStream = default(NetworkStream);
            _readData = null;

            SendMessageCommand = new ButtonCommand<object>(sendMessage, canSendMessage);
        }

        public BusinessLogic BL
        {
            get { return _startup.Bl; }
            set { _startup.Bl = value; onPopertyChange("BL"); }
        }

        public IMainWindowViewModel MainWindowViewModel
        {
            get { return _main; }
            set { _main = value; onPopertyChange("MainWindowViewModel"); }
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

        public string ReadData
        {
            get { return _readData; }
            set { setPropertyChange(ref _readData, value); }
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
                    //readData = "" + returndata;
                    ReadData = returndata.Substring(0, returndata.IndexOf("$"));
                    msg();
                }
            }
            catch (Exception)
            {

            }
        }

        public void msg()
        {
            IMainWindow window = MainWindowViewModel.getObject("window") as IMainWindow;
            if(window != null)
            {
                window.onUIThreadSync(()=> {
                    _outputMessage = _outputMessage + Environment.NewLine + " >> " + ReadData;
                });
            }
        }

        private void sendMessage(object obj)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(InputMessage + "$");
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
        }

        private bool canSendMessage(object arg)
        {
            return true;
        }

    }
}
