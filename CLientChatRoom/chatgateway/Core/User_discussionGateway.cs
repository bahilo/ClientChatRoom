using chatcommon.Entities;
using chatcommon.Interfaces;
using chatgateway.ChatRoomWebService;
using chatgateway.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace chatgateway.Core
{
    public class User_discussionGateway: IUser_discussion, INotifyPropertyChanged
    {
        private ChatRoomWebServicePortTypeClient _channel;

        public event PropertyChangedEventHandler PropertyChanged;


        public User_discussionGateway()
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

        public ChatRoomWebServicePortTypeClient User_discussionGatWayChannel
        {
            get
            {
                return _channel;
            }
        }

        public async Task<List<User_discussion>> DeleteUser_discussion(List<User_discussion> listUser_discussion)
        {
            var formatListUser_discussionToArray = listUser_discussion.User_discussionTypeToArray();
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = (await _channel.delete_data_user_discussionAsync(formatListUser_discussionToArray)).ArrayTypeToUser_discussion();
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

        public async Task<List<User_discussion>> GetUser_discussionData(int nbLine)
        {
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = (await _channel.get_data_user_discussionAsync(nbLine.ToString())).ArrayTypeToUser_discussion().OrderBy(x => x.ID).ToList();
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

        public async Task<List<User_discussion>> GetUser_discussionDataById(int id)
        {
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = (await _channel.get_data_user_discussion_by_idAsync(id.ToString())).ArrayTypeToUser_discussion();
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

        public async Task<List<User_discussion>> InsertUser_discussion(List<User_discussion> listUser_discussion)
        {
            var formatListUser_discussionToArray = listUser_discussion.User_discussionTypeToArray();
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = (await _channel.insert_data_user_discussionAsync(formatListUser_discussionToArray)).ArrayTypeToUser_discussion();
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

        public async Task<List<User_discussion>> UpdateUser_discussion(List<User_discussion> listUser_discussion)
        {
            var formatListUser_discussionToArray = listUser_discussion.User_discussionTypeToArray();
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = (await _channel.update_data_user_discussionAsync(formatListUser_discussionToArray)).ArrayTypeToUser_discussion();
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

        public async Task<List<User_discussion>> searchUser_discussion(User_discussion discussion, string filterOperator)
        {
            var formatListUser_discussionToArray = discussion.User_discussionTypeToFilterArray(filterOperator);
            List<User_discussion> result = new List<User_discussion>();
            try
            {
                result = (await _channel.get_filter_user_discussionAsync(formatListUser_discussionToArray)).ArrayTypeToUser_discussion();
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

        public async Task<List<User_discussion>> searchUser_discussionFromWebService(User_discussion item, string filterOperator)
        {
            return await searchUser_discussion(item, filterOperator);
        }

        public void Dispose()
        {
            _channel.Close();
        }
    }
}
