using chatbusiness;
using chatroom.Classes;
using chatroom.Intefaces;
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
        private List<string> _userGroupList;
        public IDiscussionViewModel _discussionViewModel;

        public UserViewModel()
        {
            _userModelList = new List<UserModel>();
            _userGroupList = new List<string>();
        }

        public UserViewModel(Func<object, object> navigation, IDiscussionViewModel discussionViewModel) : this()
        {
            this.navigation = navigation;
            _discussionViewModel = discussionViewModel;
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

        public List<string> UserGroupList
        {
            get { return _userGroupList; }
            set { setPropertyChange(ref _userGroupList, value); }
        }

        public async void load()
        {
            Dialog.showSearch("Loading...");
            UserModelList = (await BL.BLUser.GetUserData(999)).Where(x=>x.ID != BL.BLSecurity.GetAuthenticatedUser().ID && x.Username != "channel").Select(x => new UserModel { User = x }).OrderBy(x=>x.User.Status).ToList();
            var discussionList = await _discussionViewModel.retrieveUserDiscussions(BL.BLSecurity.GetAuthenticatedUser());
            UserGroupList = discussionList.Where(x=>x.UserList.Count > 1).Select(x=>x.TxtGroupName).ToList();
            Dialog.IsDialogOpen = false;
        }


    }
}
