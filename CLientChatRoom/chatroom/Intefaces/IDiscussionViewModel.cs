using chatcommon.Entities;
using chatroom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Intefaces
{
    public interface IDiscussionViewModel
    {
        Task<List<DiscussionModel>> retrieveUserDiscussions(User user);
        string generateDiscussionGroupName(int discussionId, List<UserModel> userList);
    }
}
