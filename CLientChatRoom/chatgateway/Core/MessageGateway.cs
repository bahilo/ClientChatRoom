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
    public class MessageGateway: IMessage, INotifyPropertyChanged
    {
        private ChatRoomWebServicePortTypeClient _channel;

        public event PropertyChangedEventHandler PropertyChanged;


        public MessageGateway()
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

        public ChatRoomWebServicePortTypeClient MessageGatWayChannel
        {
            get
            {
                return _channel;
            }
        }

        public async Task<List<Message>> DeleteMessage(List<Message> listMessage)
        {
            var formatListMessageToArray = listMessage.MessageTypeToArray();
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.delete_data_messageAsync(listMessage.MessageTypeToArray())).ArrayTypeToMessage();
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

        public async Task<List<Message>> GetMessageData(int nbLine)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.get_data_messageAsync(nbLine.ToString())).ArrayTypeToMessage().OrderBy(x => x.ID).ToList();
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

        public async Task<List<Message>> GetMessageDataById(int id)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.get_data_message_by_idAsync(id.ToString())).ArrayTypeToMessage();
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

        public async Task<List<Message>> GetMessageDataByDiscussionList(List<Discussion> discussionList)
        {
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.get_data_message_by_discussion_listAsync(discussionList.DiscussionTypeToArray())).ArrayTypeToMessage();
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


        public async Task<List<Message>> InsertMessage(List<Message> listMessage)
        {
            var formatListMessageToArray = listMessage.MessageTypeToArray();
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.insert_data_messageAsync(formatListMessageToArray)).ArrayTypeToMessage();
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

        public async Task<List<Message>> UpdateMessage(List<Message> listMessage)
        {
            var formatListMessageToArray = listMessage.MessageTypeToArray();
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.update_data_messageAsync(formatListMessageToArray)).ArrayTypeToMessage();
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

        public async Task<List<Message>> searchMessage(Message message, string filterOperator)
        {
            var formatListMessageToArray = message.MessageTypeToFilterArray(filterOperator);
            List<Message> result = new List<Message>();
            try
            {
                result = (await _channel.get_filter_messageAsync(formatListMessageToArray)).ArrayTypeToMessage();
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

        public async Task<List<Message>> searchMessageFromWebService(Message item, string filterOperator)
        {
            return await searchMessage(item, filterOperator);
        }

        public void Dispose()
        {
            _channel.Close();
        }
    }
}
