using System.Reflection;

namespace NotificationServiceFunction.Business.Helper
{
    public static class TimeSpanHelper
    {
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
