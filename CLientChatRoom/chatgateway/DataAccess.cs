using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatcommon.Entities;

namespace chatgateway
{
    public class DataAccess : IDataAccess
    {
        public DataAccess(IDiscussion inDiscussionGateway,
                                IUser inUserGateway,
                                    IMessage inMessageGateway,
                                        IUser_discussion inUser_discussionGateway,
                                            ISecurity inSecurityGateway)
        {
            this.DiscussionGateway = inDiscussionGateway;
            this.UserGateway = inUserGateway;
            this.MessageGateway = inMessageGateway;
            this.User_discussionGateway = inUser_discussionGateway;
            this.SecurityGateway = inSecurityGateway;
        }

        public IDiscussion DiscussionGateway { get; set; }

        public IMessage MessageGateway { get; set; }

        public IUser UserGateway { get; set; }

        public IUser_discussion User_discussionGateway { get; set; }

        public ISecurity SecurityGateway { get; set; }

        public void Dispose()
        {
            
        }

        public void SetUserCredential(User authenticatedUser)
        {
            this.DiscussionGateway.initializeCredential(authenticatedUser);
            this.UserGateway.initializeCredential(authenticatedUser);
            this.MessageGateway.initializeCredential(authenticatedUser);
            this.User_discussionGateway.initializeCredential(authenticatedUser);
        }
    }
}
