using chatcommon.Entities;
using chatcommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface IUser: IDisposable
    {
        Task<List<User>> InsertUser(List<User> userList);

        Task<List<User>> UpdateUser(List<User> userList);

        Task<List<User>> DeleteUser(List<User> userList);

        Task<List<User>> GetUserData(int nbLine);

        Task<List<User>> GetUserDataById(int id);

        Task<List<User>> GetUserDataByUser_discussionList(List<User_discussion> user_discussionList);

        Task<List<User>> searchUser(User user, EOperator filterOperator);

        void initializeCredential(User authenticatedUser);
    }
}
