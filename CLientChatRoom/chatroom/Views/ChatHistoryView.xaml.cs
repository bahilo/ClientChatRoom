﻿using chatroom.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace chatroom.Views
{
    /// <summary>
    /// Interaction logic for ChatHistoryView.xaml
    /// </summary>
    public partial class ChatHistoryView : UserControl
    {
        public ChatHistoryView()
        {
            InitializeComponent();
        }

        private void ChatRoomHistoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = FindParent.FindChildParent<Window>(this);
            if (parent != null)
            {
                this.DataContext = (MainWindowViewModel)parent.DataContext;
                ((MainWindowViewModel)this.DataContext).MessageViewModel.load();
            }
        }
    }
}
