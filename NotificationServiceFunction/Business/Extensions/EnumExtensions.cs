using System;
using System.ComponentModel;
using System.Reflection;
namespace NotificationServiceFunction.Business.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the enum value of type <typeparamref name="T"/> that has a <see cref="DescriptionAttribute"/> 
        /// matching the specified description string.
        /// </summary>
        /// <typeparam name="T">The enum type to search. Must be a struct that derives from <see cref="Enum"/>.</typeparam>
        /// <param name="description">The description string to match against the enum member's <see cref="DescriptionAttribute"/>.</param>
        /// <returns>
        /// The enum value of type <typeparamref name="T"/> that matches the specified description.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when no enum member of type <typeparamref name="T"/> has a matching <see cref="DescriptionAttribute"/>.
        /// </exception>
        /// <remarks>
        /// This method uses reflection to search for the first public static field in the enum type
        /// that has a <see cref="DescriptionAttribute"/> with a value equal to the provided <paramref name="description"/>.
        /// </remarks>

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

        /// <summary>
        /// Retrieves the <see cref="DescriptionAttribute"/> value of an enum member, 
        /// or returns the enum member's name if no description attribute is found.
        /// </summary>
        /// <param name="value">The enum value whose description is to be retrieved.</param>
        /// <returns>
        /// The string description from the <see cref="DescriptionAttribute"/>, 
        /// or the enum member name if no description is present.
        /// </returns>
        /// <remarks>
        /// This method uses reflection to inspect the <see cref="DescriptionAttribute"/> applied to the enum member.
        /// </remarks>
        
        public static string GetDescription(this Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description ?? value.ToString();
        }
    }
}
