using chatcommon.Entities;
using chatcommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface IUser_discussion: IDisposable
    {
        Task<List<User_discussion>> InsertUser_discussion(List<User_discussion> user_discussionList);

        Task<List<User_discussion>> UpdateUser_discussion(List<User_discussion> user_discussionList);

        Task<List<User_discussion>> DeleteUser_discussion(List<User_discussion> user_discussionList);

        Task<List<User_discussion>> GetUser_discussionData(int nbLine);

        Task<List<User_discussion>> GetUser_discussionDataById(int id);

        Task<List<User_discussion>> searchUser_discussion(User_discussion user_discussion, EOperator filterOperator);

        void initializeCredential(User authenticatedUser);
    }
}
