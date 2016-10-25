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

        public User User
        {
            get { return _user; }
            set { setPropertyChange(ref _user, value); }
        }

        public string TxtID
        {
            get { return _user.ID.ToString(); }
            set { _user.ID = Convert.ToInt32(value); onPropertyChange("TxtID"); }
        }

        public string TxtEmail
        {
            get { return _user.Email; }
            set { _user.Email = value; onPropertyChange("TxtEmail"); }
        }

        public string TxtFirstName
        {
            get { return _user.FirstName; }
            set { _user.FirstName = value; onPropertyChange("TxtFirstName"); }
        }

        public string TxtLastName
        {
            get { return _user.LastName; }
            set { _user.LastName= value; onPropertyChange("TxtLastName"); }
        }

        public string TxtPassword
        {
            get { return _user.Password; }
            set { _user.Password = value; onPropertyChange("TxtPassword"); }
        }

        public string TxtUserName
        {
            get { return _user.Username; }
            set { _user.Username = value; onPropertyChange("TxtUserName"); }
        }

        public string TxtStatus
        {
            get { return _user.Status.ToString(); }
            set { _user.Status = Convert.ToInt32(value); onPropertyChange("TxtStatus"); }
        }

    }
}
