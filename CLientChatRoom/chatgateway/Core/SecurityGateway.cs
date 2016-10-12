using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatcommon.Entities;
using System.ComponentModel;
using chatgateway.ChatRoomWebService;
using chatgateway.Helper;
using System.ServiceModel;

namespace chatgateway.Core
{
    public class SecurityGateway : ISecurity, INotifyPropertyChanged
    {
        private ChatRoomWebServicePortTypeClient _channel;

        public event PropertyChangedEventHandler PropertyChanged;


        public SecurityGateway()
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

        public async Task<User> AuthenticateUser(string username, string password, bool isClearPassword = true)
        {
            User agentFound = new User();
            try
            {
                setServiceCredential(username, password);
                //UserChatRoom[] agentArray = new UserChatRoom[1];
                UserChatRoom agentArray = await _channel.get_authenticate_userAsync(username, password);
                agentFound = new UserChatRoom[] { agentArray }.ArrayTypeToUser().FirstOrDefault();
                if (agentFound == null || agentFound.ID == 0)
                    throw new ApplicationException(string.Format("Your login {0} does not match any user in our database!", username));
            }
            catch (FaultException)
            {
                Dispose();
                throw;
            }
            catch (CommunicationException ex)
            {
                _channel.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                _channel.Abort();
                throw;
            }
            return agentFound;
        }

        public string CalculateHash(string clearTextPassword)
        {
            throw new NotImplementedException();
        }

        public User GetAuthenticatedUser()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
