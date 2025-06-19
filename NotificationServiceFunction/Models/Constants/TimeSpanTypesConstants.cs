using System.Reflection;

namespace NotificationServiceFunction.Models.Constants
{
    public static class TimeSpanTypesConstants
    {
        public const string Days = nameof(TimeSpan.FromDays);
        public const string Hours = nameof(TimeSpan.FromHours);
        public const string Minutes = nameof(TimeSpan.FromMinutes);
        public const string Seconds = nameof(TimeSpan.FromSeconds);
        public const string Milliseconds = nameof(TimeSpan.FromMilliseconds);
        public const string Ticks = nameof(TimeSpan.FromTicks);

        public static void IsValidTimeSpanType(string timeSpanType)
        {
            if(!typeof(TimeSpanTypesConstants).GetFields(BindingFlags.Public | BindingFlags.Static).Any(f => f.Name.Equals(timeSpanType, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"No matching TimeSpanType found for: {timeSpanType}");
        }
    }
}
