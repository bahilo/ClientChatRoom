using chatcommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbusiness
{
    public class BusinessLogic : IBusinessLogic
    {
        public BusinessLogic(IDiscussion inBLDiscussion,
                                IUser inBLUser,
                                    IMessage inBLMessage,
                                        IUser_discussion inBLUser_discussion,
                                            ISecurity inBLSecurity)
        {
            this.BLDiscussion = inBLDiscussion;
            this.BLUser = inBLUser;
            this.BLMessage = inBLMessage;
            this.BLUser_discussion = inBLUser_discussion;
            this.BLSecurity = inBLSecurity;
        }

        public IDiscussion BLDiscussion { get; set; }

        public IMessage BLMessage { get; set; }

        public IUser BLUser { get; set; }

        public IUser_discussion BLUser_discussion { get; set; }

        public ISecurity BLSecurity { get; set; }
    }
}
