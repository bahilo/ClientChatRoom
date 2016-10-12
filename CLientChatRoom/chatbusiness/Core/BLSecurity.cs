using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatcommon.Entities;
using System.Security.Cryptography;
using chatcommon.Classes;

namespace chatbusiness.Core
{
    public class BLSecurity : ISecurity
    {
        private User User;
        private IDataAccess DAC;

        public BLSecurity(IDataAccess DataAccessComponent)
        {
            DAC = DataAccessComponent;
        }

        public async Task<User> AuthenticateUser(string username, string password, bool isClearPassword = true)
        {
            try
            {
                if (isClearPassword)
                    User = await DAC.SecurityGateway.AuthenticateUser(username, CalculateHash(password));  //DAC.DALSecurity.AuthenticateUser(username, CalculateHash(password));
                else
                    User = await DAC.SecurityGateway.AuthenticateUser(username, password);  //DAC.DALSecurity.AuthenticateUser(username, CalculateHash(password));
                
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
                User = new User();
                return User;
            }
            return User;
        }

        public string CalculateHash(string clearTextPassword)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + 59);
            // Use the hash algorithm to calculate the hash
            MD5 algorithm = MD5.Create();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash string
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public User GetAuthenticatedUser()
        {
            return User;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
