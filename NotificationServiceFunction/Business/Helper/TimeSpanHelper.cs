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
        public static TimeSpan GetTimeSpan(string timeSpanType, int timeAmount)
        {
            switch(timeSpanType)
            {
                case TimeSpanConstants.FromMinutes:
                    return TimeSpan.FromMinutes(timeAmount);
                case TimeSpanConstants.FromHours:
                    return TimeSpan.FromHours(timeAmount);
                case TimeSpanConstants.FromDays:
                    return TimeSpan.FromDays(timeAmount);
                 default:
                    return TimeSpan.Zero;
            }
        }
    }
}
