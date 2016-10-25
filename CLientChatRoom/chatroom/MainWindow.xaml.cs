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
    public partial class MainWindow : Window
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
            MainWindowViewModel = new MainWindowViewModel();
            this.DataContext = MainWindowViewModel;
        }
        
        private void DialogBox_Loaded(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel.Dispose();
        }
    }
}
