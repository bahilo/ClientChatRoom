using chatbusiness;
using chatcommon.Entities;
using chatroom.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Models
{
    public class UserModel : BindBase
    {
        private User _user;

        public UserModel()
        {
            _user = new User();
        }

        public string TxtID
        {
            get { return _user.ID.ToString(); }
            set { _user.ID = Convert.ToInt32(value); onPopertyChange("TxtID"); }
        }

        public string TxtEmail
        {
            get { return _user.Email; }
            set { _user.Email = value; onPopertyChange("TxtEmail"); }
        }

        public string TxtFirstName
        {
            get { return _user.FirstName; }
            set { _user.FirstName = value; onPopertyChange("TxtFirstName"); }
        }

        public string TxtLastName
        {
            get { return _user.LastName; }
            set { _user.LastName= value; onPopertyChange("TxtLastName"); }
        }

        public string TxtPassword
        {
            get { return _user.Password; }
            set { _user.Password = value; onPopertyChange("TxtPassword"); }
        }

        public string TxtUserName
        {
            get { return _user.Username; }
            set { _user.Username = value; onPopertyChange("TxtUserName"); }
        }

    }
}
