using chatcommon.Entities;
using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using chatgateway.ChatRoomWebService;
using System.ComponentModel;
using chatgateway.Helper;

namespace chatgateway.Core
{
    public class DiscussionGateway: IDiscussion, INotifyPropertyChanged
    {
        private ChatRoomWebServicePortTypeClient _channel;

        public event PropertyChangedEventHandler PropertyChanged;


        public DiscussionGateway()
        {
            _channel = new ChatRoomWebServicePortTypeClient("ChatRoomWebServicePort");// (binding, endPoint);
        }

        public void initializeCredential(User user)
        {
            Credential = user;
        }

        public User Credential
        {
            set
            {
                setServiceCredential(value.Username, value.Password);
                onPropertyChange("Credential");
            }
        }


        public void setServiceCredential(string login, string password)
        {
            _channel.Close();
            _channel = new ChatRoomWebServicePortTypeClient("ChatRoomWebServicePort");
            _channel.ClientCredentials.UserName.UserName = login;
            _channel.ClientCredentials.UserName.Password = password;
        }

        private void onPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ChatRoomWebServicePortTypeClient DiscussionGatWayChannel
        {
            get
            {
                return _channel;
            }
        }

        public async Task<List<Discussion>> DeleteDiscussion(List<Discussion> listDiscussion)
        {
            var formatListDiscussionToArray = listDiscussion.DiscussionTypeToArray();
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.delete_data_discussionAsync(formatListDiscussionToArray)).ArrayTypeToDiscussion();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }

        public async Task<List<Discussion>> GetDiscussionData(int nbLine)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.get_data_discussionAsync(nbLine.ToString())).ArrayTypeToDiscussion().OrderBy(x => x.ID).ToList();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }

        public async Task<List<Discussion>> GetDiscussionDataById(int id)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.get_data_discussion_by_idAsync(id.ToString())).ArrayTypeToDiscussion();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }

        public async Task<List<Discussion>> GetDiscussionDataByUser_discussionList(List<User_discussion> user_discussionList)
        {
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.get_data_discussion_by_user_discussion_listAsync(user_discussionList.User_discussionTypeToArray())).ArrayTypeToDiscussion();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }


        public async Task<List<Discussion>> InsertDiscussion(List<Discussion> listDiscussion)
        {
            var formatListDiscussionToArray = listDiscussion.DiscussionTypeToArray();
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.insert_data_discussionAsync(formatListDiscussionToArray)).ArrayTypeToDiscussion();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }

        public async Task<List<Discussion>> UpdateDiscussion(List<Discussion> listDiscussion)
        {
            var formatListDiscussionToArray = listDiscussion.DiscussionTypeToArray();
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.update_data_discussionAsync(formatListDiscussionToArray)).ArrayTypeToDiscussion();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }

        public async Task<List<Discussion>> searchDiscussion(Discussion discussion, string filterOperator)
        {
            var formatListDiscussionToArray = discussion.DiscussionTypeToFilterArray(filterOperator);
            List<Discussion> result = new List<Discussion>();
            try
            {
                result = (await _channel.get_filter_discussionAsync(formatListDiscussionToArray)).ArrayTypeToDiscussion();
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
            }
            return result;
        }

        public async Task<List<Discussion>> searchDiscussionFromWebService(Discussion item, string filterOperator)
        {
            return await searchDiscussion(item, filterOperator);
        }

        public void Dispose()
        {
            _channel.Close();
        }
    }
}
