using chatcommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface IDiscussion: IDisposable
    {
        Task<List<Discussion>> InsertDiscussion(List<Discussion> discussionList);

        Task<List<Discussion>> UpdateDiscussion(List<Discussion> discussionList);

        Task<List<Discussion>> DeleteDiscussion(List<Discussion> discussionList);

        Task<List<Discussion>> GetDiscussionData(int nbLine);

        Task<List<Discussion>> GetDiscussionDataById(int id);

        Task<List<Discussion>> GetDiscussionDataByUser_discussionList(List<User_discussion> user_discussionList);

        Task<List<Discussion>> searchDiscussion(Discussion discussion, string filterOperator);

        void initializeCredential(User authenticatedUser);
    }
}
