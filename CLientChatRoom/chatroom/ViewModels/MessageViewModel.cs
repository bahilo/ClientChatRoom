using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatroom.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.ViewModels
{
    public class MessageViewModel : BindBase
    {
        public MessageViewModel()
        {
        }

        public BusinessLogic BL
        {
            get { return _startup.Bl; }
            set { _startup.Bl = value; onPopertyChange("BL"); }
        }
    }
}
