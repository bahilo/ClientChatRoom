using chatcommon.Entities;
using chatcommon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface IMessage: IDisposable
    {
        Task<List<Message>> InsertMessage(List<Message> discussionList);

        Task<List<Message>> UpdateMessage(List<Message> discussionList);

        Task<List<Message>> DeleteMessage(List<Message> discussionList);

        Task<List<Message>> GetMessageData(int nbLine);

        Task<List<Message>> GetMessageDataById(int id);

        Task<List<Message>> searchMessage(Message message, EOperator filterOperator);

        void initializeCredential(User authenticatedUser);
    }
}
