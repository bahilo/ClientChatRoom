using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatcommon.Entities;
using chatcommon.Classes;

namespace chatbusiness.Core
{
    public class BLDiscussion : IDiscussion
    {
        public IDataAccess DAC { get; set; }

        public BLDiscussion(IDataAccess DataAccessComponent)
        {
            DAC = DataAccessComponent;
        }

        public async Task<List<Discussion>> InsertDiscussion(List<Discussion> discussionList)
        {
            if (discussionList == null || discussionList.Count == 0)
                return new List<Discussion>();

            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.InsertDiscussion(discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Discussion>> UpdateDiscussion(List<Discussion> discussionList)
        {
            if (discussionList == null || discussionList.Count == 0)
                return new List<Discussion>();

            if (discussionList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Updating discussions(count = " + discussionList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.UpdateDiscussion(discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Discussion>> DeleteDiscussion(List<Discussion> discussionList)
        {
            if (discussionList == null || discussionList.Count == 0)
                return new List<Discussion>();

            if (discussionList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Deleting discussions(count = " + discussionList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.DeleteDiscussion(discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Discussion>> GetDiscussionData(int nbLine)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.GetDiscussionData(nbLine);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Discussion>> GetDiscussionDataById(int id)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.GetDiscussionDataById(id);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }
        
        public async Task<List<Discussion>> searchDiscussion(Discussion discussion, string filterOperator)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.searchDiscussion(discussion, filterOperator);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Discussion>> GetDiscussionDataByUser_discussionList(List<User_discussion> user_discussionList)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = await DAC.DiscussionGateway.GetDiscussionDataByUser_discussionList(user_discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public void Dispose()
        {
            DAC.DiscussionGateway.Dispose();
        }

        public void initializeCredential(User authenticatedUser)
        {
            DAC.SetUserCredential(authenticatedUser);
        }

    }
}
