using System.ComponentModel;

namespace NotificationServiceFunction.Models.Enums
{
    public enum TimeSpansEnum
    {
        [Description("Minutes")]
        FromMinutes = 0,
        [Description("Hours")]
        FromHours = 1,
        [Description("Days")]
        FromDays = 2,
    }
}
