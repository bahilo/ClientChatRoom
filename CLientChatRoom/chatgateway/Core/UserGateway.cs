using chatcommon.Entities;
using chatcommon.Enums;
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
    public class UserGateway: IUser, INotifyPropertyChanged
    {
        private ChatRoomWebServicePortTypeClient _channel;
        private User AuthenticatedUser;

        public event PropertyChangedEventHandler PropertyChanged;


        public UserGateway()
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
                AuthenticatedUser = value;
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

        public ChatRoomWebServicePortTypeClient UserGatWayChannel
        {
            get
            {
                return _channel;
            }
        }

        public async Task<List<User>> DeleteUser(List<User> listUser)
        {
            var formatListUserToArray = listUser.UserTypeToArray();
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.delete_data_userAsync(formatListUserToArray)).ArrayTypeToUser();
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

        public async Task<List<User>> GetUserData(int nbLine)
        {
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.get_data_userAsync(nbLine.ToString())).ArrayTypeToUser().OrderBy(x => x.ID).ToList();
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

        public async Task<List<User>> GetUserDataById(int id)
        {
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.get_data_user_by_idAsync(id.ToString())).ArrayTypeToUser();
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

        public async Task<List<User>> GetUserDataByUser_discussionList(List<User_discussion> user_discussionList)
        {
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.get_data_user_by_user_discussion_listAsync(user_discussionList.User_discussionTypeToArray())).ArrayTypeToUser();
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


        public async Task<List<User>> InsertUser(List<User> listUser)
        {
            var formatListUserToArray = listUser.UserTypeToArray();
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.insert_data_userAsync(formatListUserToArray)).ArrayTypeToUser();
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

        public async Task<List<User>> UpdateUser(List<User> listUser)
        {
            var formatListUserToArray = listUser.UserTypeToArray();
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.update_data_userAsync(formatListUserToArray)).ArrayTypeToUser();
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

        public async Task<List<User>> searchUser(User user, EOperator filterOperator)
        {
            var formatListUserToArray = user.UserTypeToFilterArray(filterOperator.ToString());
            List<User> result = new List<User>();
            try
            {
                result = (await _channel.get_filter_userAsync(formatListUserToArray)).ArrayTypeToUser();
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

        public async Task<List<User>> searchUserFromWebService(User item, EOperator filterOperator)
        {
            return await searchUser(item, filterOperator);
        }

        public void Dispose()
        {
            _channel.Close();
        }
    }
}
