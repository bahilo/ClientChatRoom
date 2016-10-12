using chatcommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface ISecurity: IDisposable
    {
        User GetAuthenticatedUser();

        Task<User> AuthenticateUser(string username, string password, bool isClearPassword = true);
        
        string CalculateHash(string clearTextPassword);
    }
}
