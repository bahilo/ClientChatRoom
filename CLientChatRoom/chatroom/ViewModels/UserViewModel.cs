using chatbusiness;
using chatroom.Classes;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace chatroom.ViewModels
{
    public class UserViewModel : BindBase
    {
        private Func<object, object> navigation;
        private List<UserModel> _userModelList;

        public UserViewModel()
        {
            _userModelList = new List<UserModel>();
        }

        public UserViewModel(Func<object, object> navigation) : this()
        {
            this.navigation = navigation;
        }

        public BusinessLogic BL
        {
            get { return _startup.BL; }
            set { _startup.BL = value; onPropertyChange("BL"); }
        }

        public List<UserModel> UserModelList
        {
            get { return _userModelList; }
            set { setPropertyChange(ref _userModelList, value); }
        }

        public async void load()
        {
            UserModelList = (await BL.BLUser.GetUserData(999)).Where(x=>x.ID != BL.BLSecurity.GetAuthenticatedUser().ID && x.Username != "channel").Select(x => new UserModel { User = x }).OrderBy(x=>x.User.Status).ToList();
        }
    }
}
