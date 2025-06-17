using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceFunction.Models
{
    public class NotificationRateLimits
    {
        //TODO: Replace with config file json or ideally read from DB or azure table
        public static readonly Dictionary<string, (int Limit, TimeSpan Period)> Limits = new()
        {
            { "Status", (2, TimeSpan.FromMinutes(1)) },
            { "News", (1, TimeSpan.FromDays(1)) },
            { "Marketing", (3, TimeSpan.FromHours(1)) }
        };
    }
}
