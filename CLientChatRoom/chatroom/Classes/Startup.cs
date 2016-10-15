using chatbusiness;
using chatbusiness.Core;
using chatcommon.Classes;
using chatcommon.Entities;
using chatgateway;
using chatgateway.Classes;
using chatgateway.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Classes
{
    public class Startup
    {
        public BusinessLogic Bl { get; set; }
        public DataAccess Dal { get; set; }

        public Startup()
        {
            initialize();
        }

        public async void initialize()
        {
            Dal = new DataAccess(
                                new DiscussionGateway(),
                                new UserGateway(),
                                new MessageGateway(),
                                new User_discussionGateway(),
                                new SecurityGateway());

            BLSecurity BLSecurity = new BLSecurity(Dal);
            Dal.SetUserCredential(await BLSecurity.AuthenticateUser(WebCommunication.ChannelUserName, WebCommunication.ChannelPassword, false));
            Bl = new BusinessLogic(
                                new BLDiscussion(Dal),
                                new BLUser(Dal),
                                new BLMessage(Dal),
                                new BLUser_discussion(Dal),
                                BLSecurity);
        }
    }
}
