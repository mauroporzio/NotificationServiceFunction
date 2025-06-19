using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceFunction.Models.Enum
{
    public enum NotificationStatusEnum
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Delivered = 3,
        Cancelled = 4
    }
}
