using chatcommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface IDataAccess: IDisposable
    {
        IUser UserGateway { get; set; }
        IMessage MessageGateway { get; set; }
        IDiscussion DiscussionGateway { get; set; }
        IUser_discussion User_discussionGateway { get; set; }
        ISecurity SecurityGateway { get; set; }
        void SetUserCredential(User authenticatedUser);
    }
}
