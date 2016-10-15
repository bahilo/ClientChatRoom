using chatroom.Classes;
using chatroom.ViewModels;
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
    /// Interaction logic for SecuritySignUpView.xaml
    /// </summary>
    public partial class SecuritySignUpView : UserControl
    {
        public SecuritySignUpView()
        {
            InitializeComponent();
        }

        private void UserSignUpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var parent = FindParent.FindChildParent<Window>(this);
            //if (parent != null)
            //{
                //this.DataContext = (MainWindowViewModel)parent.DataContext;
                pwdBox.Password = ((MainWindowViewModel)this.DataContext).SecurityLoginViewModel.UserModel.TxtPassword;
                pwdBoxVerification.Password = ((MainWindowViewModel)this.DataContext).SecurityLoginViewModel.UserModel.TxtPassword;
                pwdBox.LostFocus += ((MainWindowViewModel)this.DataContext).SecurityLoginViewModel.onPwdBoxPasswordChange_updateTxtClearPassword;
                pwdBoxVerification.LostFocus += ((MainWindowViewModel)this.DataContext).SecurityLoginViewModel.onPwdBoxVerificationPasswordChange_updateTxtClearPasswordVerification;
            //}
        }
    }
}
