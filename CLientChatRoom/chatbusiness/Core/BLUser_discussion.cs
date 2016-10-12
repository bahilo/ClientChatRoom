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
    public class BLUser_discussion: IUser_discussion
    {
        public IDataAccess DAC { get; set; }

        public BLUser_discussion(IDataAccess DataAccessComponent)
        {
            DAC = DataAccessComponent;
        }

        public async Task<List<User_discussion>> InsertUser_discussion(List<User_discussion> user_discussionList)
        {
            if (user_discussionList == null || user_discussionList.Count == 0)
                return new List<User_discussion>();

            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = await DAC.User_discussionGateway.InsertUser_discussion(user_discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User_discussion>> UpdateUser_discussion(List<User_discussion> user_discussionList)
        {
            if (user_discussionList == null || user_discussionList.Count == 0)
                return new List<User_discussion>();

            if (user_discussionList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Updating user_discussions(count = " + user_discussionList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = await DAC.User_discussionGateway.UpdateUser_discussion(user_discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User_discussion>> DeleteUser_discussion(List<User_discussion> user_discussionList)
        {
            if (user_discussionList == null || user_discussionList.Count == 0)
                return new List<User_discussion>();

            if (user_discussionList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Deleting user_discussions(count = " + user_discussionList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = await DAC.User_discussionGateway.DeleteUser_discussion(user_discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User_discussion>> GetUser_discussionData(int nbLine)
        {
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = await DAC.User_discussionGateway.GetUser_discussionData(nbLine);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User_discussion>> GetUser_discussionDataById(int id)
        {
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = await DAC.User_discussionGateway.GetUser_discussionDataById(id);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<User_discussion>> searchUser_discussion(User_discussion user_discussion_discussion, string filterOperator)
        {
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = await DAC.User_discussionGateway.searchUser_discussion(user_discussion_discussion, filterOperator);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public void Dispose()
        {
            DAC.User_discussionGateway.Dispose();
        }

        public void initializeCredential(User authenticatedUser)
        {
            DAC.SetUserCredential(authenticatedUser);
        }
    }
}
