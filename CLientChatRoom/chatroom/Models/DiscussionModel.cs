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
    public class DiscussionModel : BindBase
    {
        private Discussion _discussion;

        public DiscussionModel()
        {
            _discussion = new Discussion();
        }

        public string TxtID
        {
            get { return _discussion.ID.ToString(); }
            set { _discussion.ID = Convert.ToInt32(value); onPropertyChange("TxtID"); }
        }

        public string TxtDate
        {
            get { return _discussion.Date.ToString(); }
            set { _discussion.Date = Utility.convertToDateTime(value); onPropertyChange("TxtDate"); }
        }
    }

    
}
