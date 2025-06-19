using NotificationServiceFunction.Models;
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
                case TimeSpanConstants.FromMinutes:
                    return TimeSpan.FromMinutes(1);
                case TimeSpanConstants.FromHours:
                    return TimeSpan.FromHours(1);
                case TimeSpanConstants.FromDays:
                    return TimeSpan.FromDays(1);
                 default:
                    return TimeSpan.Zero;
            }
        }
    }
}
