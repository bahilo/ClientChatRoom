using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatcommon.Entities
{
    public class Message
    {
        public int ID { get; set; }

        public int DiscussionId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public string Content { get; set; }
    }
}
