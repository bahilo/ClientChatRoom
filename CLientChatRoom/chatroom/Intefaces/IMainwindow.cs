using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Intefaces
{
    public interface IMainWindow
    {
        Task onUIThreadAsync(System.Action action);
        void onUIThreadSync(System.Action action);
        void showRecipientReply(string message);
        void showMyReply(string message);
        void showInfo(string message);
    }
}
