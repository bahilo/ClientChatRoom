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
            set { _message.ID = Convert.ToInt32(value); onPropertyChange("TxtID"); }
        }

        public string TxtDiscussion
        {
            get { return _message.DiscussionId.ToString(); }
            set { _message.DiscussionId = Convert.ToInt32(value); onPropertyChange("TxtDiscussion"); }
        }

        public string TxtDate
        {
            get { return _message.Date.ToString(); }
            set { _message.Date = Utility.convertToDateTime(value); onPropertyChange("TxtDate"); }
        }

        public string TxtContent
        {
            get { return _message.Content; }
            set { _message.Content = value; onPropertyChange("TxtContent"); }
        }
    }
}
