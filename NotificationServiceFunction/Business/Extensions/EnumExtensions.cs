using System;
using System.ComponentModel;
using System.Reflection;
namespace NotificationServiceFunction.Business.Extensions
{
    public static class EnumExtensions
    {
        public static T? FromDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();

                if (attribute?.Description == description)
                {
                    return (T)field.GetValue(null)!;
                }
            }

            throw new ArgumentException($"No matching enum value found for description '{description}'", nameof(description));
        }

        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description ?? value.ToString();
        }
    }
}
