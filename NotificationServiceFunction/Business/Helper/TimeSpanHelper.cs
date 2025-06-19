using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceFunction.Business.Helper
{
    public static class TimeSpanHelper
    {
        public static TimeSpan GetTimeSpan(string timeSpanType)
        {
            switch(timeSpanType)
            {
                case "Minutes":
                    return TimeSpan.FromMinutes(1);
                case "Hours":
                    return TimeSpan.FromHours(1);
                case "Days":
                    return TimeSpan.FromDays(1);
                 default:
                    return TimeSpan.Zero;
            }
        }
    }
}
