using chatbusiness;
using chatcommon.Classes;
using chatcommon.Entities;
using chatroom.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatroom.Models
{
    public class MessageModel : BindBase
    {
        private Message _message;

        public MessageModel()
        {
            _message = new Message();
        }

        public string TxtID
        {
            get { return _message.ID.ToString(); }
            set { _message.ID = Convert.ToInt32(value); onPopertyChange("TxtID"); }
        }

        public string TxtDate
        {
            get { return _message.Date.ToString(); }
            set { _message.Date = Utility.convertToDateTime(value); onPopertyChange("TxtDate"); }
        }

        public string TxtContent
        {
            get { return _message.Content; }
            set { _message.Content = value; onPopertyChange("TxtContent"); }
        }
    }
}
