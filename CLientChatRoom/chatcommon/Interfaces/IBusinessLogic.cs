using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Interfaces
{
    public interface IBusinessLogic
    {
        IDiscussion BLDiscussion { get; set; }
        IMessage BLMessage { get; set; }
        IUser BLUser { get; set; }
        IUser_discussion BLUser_discussion { get; set; }
        ISecurity BLSecurity { get; set; }

    }
}
