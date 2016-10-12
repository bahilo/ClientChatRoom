using chatroom.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chatroom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        MainWindowViewModel MainWindowViewModel;
        //System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        //NetworkStream serverStream = default(NetworkStream);
        //string readData = null;

        public MainWindow()
        {
            MainWindowViewModel = new MainWindowViewModel(this);
            this.DataContext = MainWindowViewModel;

            InitializeComponent();
        }

        /*private void getMessage()
        {
            try
            {
                while (true)
                {
                    serverStream = clientSocket.GetStream();
                    int buffSize = 0;
                    buffSize = clientSocket.ReceiveBufferSize;
                    byte[] inStream = new byte[buffSize];
                    serverStream.Read(inStream, 0, buffSize);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    //readData = "" + returndata;
                    readData = returndata.Substring(0, returndata.IndexOf("$"));
                    msg();
                }
            }
            catch (Exception)
            {

            }
        }

        private void msg()
        {
            this.Dispatcher.Invoke(new Action(() => {
                textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
            }));
        }

        private void btn_server_connect_Click(object sender, RoutedEventArgs e)
        {
            readData = "Connected to Chat Server ...";
            msg();

            try
            {
                clientSocket.Connect("127.0.0.1", 8888);
                serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("Test" + "$");//textBox3.Text
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(getMessage);
                ctThread.Start();
            }
            catch (Exception)
            {
                readData = "You are already connected!";
                msg();
            }

        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox2.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        private void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }*/

        public async Task onUIThreadAsync(Action action)
        {
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, action);
        }

        public void onUIThreadSync(Action action)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, action);
        }
    }
}
