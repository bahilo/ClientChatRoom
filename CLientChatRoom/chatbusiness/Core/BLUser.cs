using chatcommon.Classes;
using chatcommon.Entities;
using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbusiness.Core
{
    public class BLUser: IUser
    {
        public IDataAccess DAC { get; set; }

        public BLUser(IDataAccess DataAccessComponent)
        {
            DAC = DataAccessComponent;
        }

        public async Task<List<User>> InsertUser(List<User> userList)
        {
            if (userList == null || userList.Count == 0)
                return new List<User>();

            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.InsertUser(userList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User>> UpdateUser(List<User> userList)
        {
            if (userList == null || userList.Count == 0)
                return new List<User>();

            if (userList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Updating users(count = " + userList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.UpdateUser(userList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User>> DeleteUser(List<User> userList)
        {
            if (userList == null || userList.Count == 0)
                return new List<User>();

            if (userList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Deleting users(count = " + userList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.DeleteUser(userList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User>> GetUserData(int nbLine)
        {
            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.GetUserData(nbLine);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User>> GetUserDataById(int id)
        {
            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.GetUserDataById(id);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User>> searchUser(User user, string filterOperator)
        {
            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.searchUser(user, filterOperator);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User>> GetUserDataByUser_discussionList(List<User_discussion> user_discussionList)
        {
            List<User> result = new List<User>();
            try
            {
                result = await DAC.UserGateway.GetUserDataByUser_discussionList(user_discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public void Dispose()
        {
            DAC.UserGateway.Dispose();
        }

        public void initializeCredential(User authenticatedUser)
        {
            DAC.SetUserCredential(authenticatedUser);
        }
    }
}
