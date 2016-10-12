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
    public class BLMessage: IMessage
    {
        public IDataAccess DAC { get; set; }

        public BLMessage(IDataAccess DataAccessComponent)
        {
            DAC = DataAccessComponent;
        }

        public async Task<List<Message>> InsertMessage(List<Message> messageList)
        {
            if (messageList == null || messageList.Count == 0)
                return new List<Message>();

            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.InsertMessage(messageList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Message>> UpdateMessage(List<Message> messageList)
        {
            if (messageList == null || messageList.Count == 0)
                return new List<Message>();

            if (messageList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Updating messages(count = " + messageList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.UpdateMessage(messageList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Message>> DeleteMessage(List<Message> messageList)
        {
            if (messageList == null || messageList.Count == 0)
                return new List<Message>();

            if (messageList.Where(x => x.ID == 0).Count() > 0)
                Log.write("Deleting messages(count = " + messageList.Where(x => x.ID == 0).Count() + ") with ID = 0", "WAR");

            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.DeleteMessage(messageList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Message>> GetMessageData(int nbLine)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.GetMessageData(nbLine);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Message>> GetMessageDataById(int id)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.GetMessageDataById(id);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Message>> searchMessage(Message message, string filterOperator)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.searchMessage(message, filterOperator);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public async Task<List<Message>> GetMessageDataByDiscussionList(List<Discussion> discussionList)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = await DAC.MessageGateway.GetMessageDataByDiscussionList(discussionList);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message);
            }
            return result;
        }

        public void Dispose()
        {
            DAC.MessageGateway.Dispose();
        }

        public void initializeCredential(User authenticatedUser)
        {
            DAC.SetUserCredential(authenticatedUser);
        }
    }
}
