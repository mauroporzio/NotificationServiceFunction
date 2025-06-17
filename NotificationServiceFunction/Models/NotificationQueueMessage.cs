using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceFunction.Models
{
    public class NotificationQueueMessage
    {
        public string Recipient { get; set; }
        public string NotificationType { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string Content { get; set; }
    }
}
