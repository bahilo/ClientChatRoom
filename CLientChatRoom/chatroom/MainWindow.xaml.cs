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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace chatroom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel MainWindowViewModel;
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
