using NotificationServiceFunction.Business.Extensions;
using NotificationServiceFunction.Models.Enums;

namespace NotificationServiceFunction.Business.Helper
{
    public static class TimeSpanHelper
    {
        public static TimeSpan GetTimeSpan(string timeSpanType, int timeAmount)
        {
            TimeSpansEnum spanEnum = EnumExtensions.FromDescription<TimeSpansEnum>(timeSpanType);

            switch (spanEnum)
            {
                case TimeSpansEnum.FromMinutes:
                    return TimeSpan.FromMinutes(timeAmount);
                case TimeSpansEnum.FromHours:
                    return TimeSpan.FromHours(timeAmount);
                case TimeSpansEnum.FromDays:
                    return TimeSpan.FromDays(timeAmount);
                 default:
                    throw new Exception($"No matching TimeSpan handling found for type: '{timeSpanType}'.");
            }
        }
    }
}
