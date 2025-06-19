using System.Reflection;

namespace NotificationServiceFunction.Business.Helper
{
    public static class TimeSpanHelper
    {
        /// <summary>
        /// Returns a <see cref="TimeSpan"/> based on the given time span type and amount.
        /// The type must match a valid static method on <see cref="TimeSpan"/>, like "Minutes", "Hours", etc.
        /// </summary>
        /// <param name="timeSpanType">The name of the time span unit (e.g., "Minutes", "Hours").</param>
        /// <param name="timeAmount">The numeric value for the specified time span type.</param>
        /// <returns>A <see cref="TimeSpan"/> representing the given amount and type.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided type is invalid.</exception>

        public static TimeSpan GetTimeSpan(string timeSpanType, int timeAmount)
        {
            // Build method name.
            string methodName = $"From{timeSpanType}";

            // Get the static method from TimeSpan that matches the name
            MethodInfo? method = typeof(TimeSpan).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);

            if (method == null)
                throw new ArgumentException($"Invalid time span type: {timeSpanType}");

            // Call the method with the timeAmount
            object? result = method.Invoke(null, new object[] { Convert.ToDouble(timeAmount) });

            return (TimeSpan)result!;
        }
    }
}
