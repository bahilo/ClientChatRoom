using chatroom.Classes;
using System.Windows;
using System.Windows.Controls;

namespace chatroom.Views
{
    /// <summary>
    /// Interaction logic for ConfirmationView.xaml
    /// </summary>
    public partial class ConfirmationView : UserControl
    {
        public ConfirmationView()
        {
            InitializeComponent();
        }

        private void ConfirmationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = FindParent.FindChildParent<Window>(this);
            if (parent != null)
            {
                this.DataContext = (MainWindowViewModel)parent.DataContext;
            }
        }
    }
}
