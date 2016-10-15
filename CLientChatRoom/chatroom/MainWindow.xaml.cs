using chatcommon.Classes;
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

        public MainWindow()
        {
            InitializeComponent();
            //showInfo("Server connected.");
            //showRecipientReply("here is the first test!pppppppppppppplllllllllllllllllllllllllllllllllllllllllllllllll" + Environment.NewLine + "totototottoottotot");
            //showMyReply("here is the first testooooooooooooooooooooooooooooooooooooooooooooo!" + Environment.NewLine + "totototottoottotot");
        }

        private void load()
        {
            MainWindowViewModel = new MainWindowViewModel(this);
            this.DataContext = MainWindowViewModel;
        }


        public async Task onUIThreadAsync(Action action)
        {
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, action);
        }

        public void onUIThreadSync(Action action)
        {
            try
            {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, action);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
        }

        private void DialogBox_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        public void showMyReply(string message)
        {
            Button btnMessage = new Button();
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = message;
            btnMessage.Style = (Style)FindResource("Reply");
            btnMessage.Content = txtBlock;

            onUIThreadSync(() =>
            {
                chatRoomZone.Children.Add(btnMessage);
            });
        }

        public void showInfo(string message)
        {
            TextBlock txtBlock = new TextBlock();
            txtBlock.HorizontalAlignment = HorizontalAlignment.Center;
            txtBlock.Text = message;
            onUIThreadSync(() =>
            {
                chatRoomZone.Children.Add(txtBlock);
            });
        }

        public async void showRecipientReply(string message)
        {     
            await onUIThreadAsync(() =>
            {
                Button btnMessage = new Button();
                TextBlock txtBlock = new TextBlock();
                txtBlock.Text = message;
                btnMessage.Style = (Style)FindResource("RecipientReply");
                btnMessage.Content = txtBlock;
                chatRoomZone.Children.Add(btnMessage);
            });
        }
    }
}
