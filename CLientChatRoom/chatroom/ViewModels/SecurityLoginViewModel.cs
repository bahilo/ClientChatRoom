using chatbusiness;
using chatcommon.Classes;
using chatcommon.Enums;
using chatroom.Classes;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace chatroom.ViewModels
{
    public class SecurityLoginViewModel : BindBase
    {
        private string _clearPassword;
        private string _login;
        private string _errorMessage;
        private UserModel _userModel;
        private string _clearPasswordVerification;
        private NotifyTaskCompletion<int> _DialogtaskCompletion;
        private NotifyTaskCompletion<int> _signUpTaskCompletion;
        private NotifyTaskCompletion<object> _authenticateUsertaskCompletion;
        private Func<object, object> _page;

        public SecurityLoginViewModel()
        {
            _errorMessage = "";
            _clearPassword = "";
            _clearPasswordVerification = "";
            _userModel = new UserModel();
            _DialogtaskCompletion = new NotifyTaskCompletion<int>();
            _signUpTaskCompletion = new NotifyTaskCompletion<int>();
            _authenticateUsertaskCompletion = new NotifyTaskCompletion<object>();

            _DialogtaskCompletion.PropertyChanged += onDialogDisplayTaskComplete_authenticateUser;
            _signUpTaskCompletion.PropertyChanged += onSignUpTaskComplete_checkIfUserFound;
            _authenticateUsertaskCompletion.PropertyChanged += onAuthenticateUserTaskComplete_checkIfUserExist;
        }

        public SecurityLoginViewModel(Func<object, object> navigation): this()
        {
            this._page = navigation;
        }

        public UserModel UserModel
        {
            get { return _userModel; }
            set { setPropertyChange(ref _userModel, value); }
        }

        public BusinessLogic Bl
        {
            get { return _startup.BL; }
            set { _startup.BL = value; onPropertyChange("Bl"); }
        }

        public string TxtErrorMessage
        {
            get { return _errorMessage; }
            set { setPropertyChange(ref _errorMessage, value); }
        }

        public string TxtClearPassword
        {
            get { return _clearPassword; }
            set { _clearPassword = value; onPropertyChange("TxtClearPassword"); }
        }

        public string TxtUserName
        {
            get { return _login; }
            set { setPropertyChange(ref _login, value); }
        }

        public string TxtClearPasswordVerification
        {
            get { return _clearPasswordVerification; }
            set { setPropertyChange(ref _clearPasswordVerification, value); }
        }
        
        internal void onPwdBoxPasswordChange_updateTxtClearPassword(object sender, RoutedEventArgs e)
        {
            PasswordBox pwd = ((PasswordBox)sender);
            if (pwd.Password.Count() > 0)
                TxtClearPassword = pwd.Password;
        }

        internal void onPwdBoxVerificationPasswordChange_updateTxtClearPasswordVerification(object sender, RoutedEventArgs e)
        {
            PasswordBox pwd = ((PasswordBox)sender);
            if (pwd.Password.Count() > 0)
            {
                TxtClearPasswordVerification = pwd.Password;
                if (TxtClearPassword.Equals(TxtClearPasswordVerification))
                {
                    UserModel.TxtPassword = Bl.BLSecurity.CalculateHash(TxtClearPassword);
                }                                    
                else
                {
                    TxtErrorMessage ="Password are not Identical!";
                    showSignUp();
                }
            }
        }

        private void onSignUpTaskComplete_checkIfUserFound(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSuccessfullyCompleted") && Dialog.Response == 1)
            {
                Dialog.Response = 0;
                signUp();
            }
        }

        private void onDialogDisplayTaskComplete_authenticateUser(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("IsSuccessfullyCompleted"))
            {
                int result = _DialogtaskCompletion.Result;
                if (!string.IsNullOrEmpty(TxtUserName) && !string.IsNullOrEmpty(TxtClearPassword) && result == 1)
                    _authenticateUsertaskCompletion.initializeNewTask(authenticateAgent(TxtUserName, TxtClearPassword));
                else if (result == 2)
                    showSignUp();                
                else
                    showView();
            }
        }

        private void onAuthenticateUserTaskComplete_checkIfUserExist(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSuccessfullyCompleted"))
            {
                if (UserModel.User.ID == 0)
                {
                    showView();
                }
            }
        }


        public void showView()
        {
            _DialogtaskCompletion.initializeNewTask(Dialog.show(new Views.SecurityLoginView()));
            
        }

        public void showSignUp()
        {
            _signUpTaskCompletion.initializeNewTask(Dialog.show(new Views.SecuritySignUpView()));
        }

        private async void signUp()
        {
            var userFoundList = await Bl.BLUser.searchUser(new chatcommon.Entities.User { Username = UserModel.TxtUserName }, EOperator.AND);
            if (userFoundList.Count == 0 && !string.IsNullOrEmpty(TxtClearPassword) && TxtClearPasswordVerification.Equals(TxtClearPassword))
            {
                var userSavedList = await Bl.BLUser.InsertUser(new List<chatcommon.Entities.User> { UserModel.User });
                if (userSavedList.Count > 0)                
                    await authenticateAgent(userSavedList[0].Username, userSavedList[0].Password, false); 
            }
            else if(userFoundList.Count != 0)
            {
                TxtErrorMessage = "This user name is already taken!";
                showSignUp();
            }                
            else
            {
                TxtErrorMessage = "Password are not Identicals or empty!";
                showSignUp();
            } 
        }

        public async Task<object> authenticateAgent(string username, string password, bool isClearPassword = true)
        {
            //Dialog.showSearch("Searching...");
            var userFound = await Bl.BLSecurity.AuthenticateUser(username, password, isClearPassword);
            if (userFound != null && userFound.ID != 0)
            {
                UserModel.User = userFound;
                _page(new MessageViewModel());
                TxtUserName = "";
                TxtClearPassword = "";
                TxtErrorMessage = "";
            }
            else
            {
                TxtErrorMessage = "Your User Name or password is incorrect!";
                showView();
            }

            return null;
            //Dialog.IsDialogOpen = false;
        }
        
    }
}
