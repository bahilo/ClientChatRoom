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
        private bool _isNewMesage;

        public MessageModel()
        {
            _message = new Message();
        }

        public Message Message
        {
            get { return _message; }
            set { setPropertyChange(ref _message, value); }
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

        public string TxtUserId
        {
            get { return _message.UserId.ToString(); }
            set { _message.UserId = Convert.ToInt32(value); onPropertyChange("TxtUserId"); }
        }

        public string TxtDate
        {
            get { return _message.Date.ToString(); }
            set { _message.Date = Utility.convertToDateTime(value); onPropertyChange("TxtDate"); }
        }

        public bool IsNewMessage
        {
            get { return _isNewMesage; }
            set { setPropertyChange(ref _isNewMesage, value); }
        }

        public string TxtContent
        {
            get { return _message.Content; }
            set { _message.Content = value; onPropertyChange("TxtContent"); }
        }

        public string TxtStatus
        {
            get { return _message.Status.ToString(); }
            set { _message.Status = Convert.ToInt32(value); onPropertyChange("TxtStatus"); }
        }
    }
}
