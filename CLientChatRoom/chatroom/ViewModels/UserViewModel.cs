using chatbusiness;
using chatroom.Classes;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.ViewModels
{
    public class UserViewModel : BindBase
    {

        private UserModel _userModel;

        public UserViewModel()
        {

        }

        public BusinessLogic BL
        {
            get { return _startup.Bl; }
            set { _startup.Bl = value; onPopertyChange("BL"); }
        }

        public UserModel UserModel { get; set; }
    }
}
