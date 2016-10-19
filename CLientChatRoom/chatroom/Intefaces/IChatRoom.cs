using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Intefaces
{
    public interface IChatRoom
    {
        void showRecipientReply(string message, bool isNewDiscussion = false);
        void showMyReply(string message, bool isNewDiscussion = false);
        void showInfo(string message);
    }
}
