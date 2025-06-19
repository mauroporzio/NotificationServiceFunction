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


        /// <summary>
        /// Validates whether the provided time span type string corresponds to a defined constant in <see cref="TimeSpanTypesConstants"/>.
        /// </summary>
        /// <param name="timeSpanType">The name of the time span type to validate (e.g., "Minutes", "Hours").</param>
        /// <exception cref="Exception">
        /// Thrown when the provided <paramref name="timeSpanType"/> does not match any constant defined in <see cref="TimeSpanTypesConstants"/>.
        /// </exception>
        /// <remarks>
        /// This method uses reflection to dynamically inspect the public static fields of <see cref="TimeSpanTypesConstants"/>.
        /// It ensures the provided string matches one of the available time span type keys (e.g., "Minutes").
        /// The comparison is case-insensitive.
        /// </remarks>

        public static void IsValidTimeSpanType(string timeSpanType)
        {
            if(!typeof(TimeSpanTypesConstants).GetFields(BindingFlags.Public | BindingFlags.Static).Any(f => f.Name.Equals(timeSpanType, StringComparison.OrdinalIgnoreCase)))
                throw new Exception($"No matching TimeSpanType found for: {timeSpanType}");
        }
    }
}
