using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatroom.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Models
{
    public class DiscussionModel : BindBase
    {
        private Discussion _discussion;
        private List<UserModel> _userList;
        private bool _isGroupDiscussion;
        private string _groupName;

        public DiscussionModel()
        {
            _discussion = new Discussion();
            _userList = new List<UserModel>();
        }
        

        public Discussion Discussion
        {
            get { return _discussion; }
            set { setPropertyChange(ref _discussion, value); }
        }


        public bool IsGroupDiscussion
        {
            get { return _isGroupDiscussion; }
            set { setPropertyChange(ref _isGroupDiscussion, value); }
        }

        public List<UserModel> UserList
        {
            get { return _userList; }
            set { setPropertyChange(ref _userList, value); }
        }

        public string TxtGroupName
        {
            get { return _groupName; }
            set { setPropertyChange(ref _groupName, value); }
        }

        public string TxtID
        {
            get { return _discussion.ID.ToString(); }
            set { _discussion.ID = Convert.ToInt32(value); onPropertyChange("TxtID"); }
        }

        public string TxtDate
        {
            get { return _discussion.Date.ToString(); }
            set { _discussion.Date = Utility.convertToDateTime(value); onPropertyChange("TxtDate"); }
        }

        public void addUser(UserModel userModel)
        {
            if (UserList.Where(x => x.User.ID == userModel.User.ID).Count() == 0)
                UserList.Add(userModel);

            if (UserList.Count > 0)
                IsGroupDiscussion = true;
        }

        public void addUser(List<UserModel> userModelList)
        {
            foreach (UserModel userModel in userModelList)
            {
                addUser(userModel);
            }
        }

    }

    
}
