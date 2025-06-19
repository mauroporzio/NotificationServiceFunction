using System.ComponentModel;

namespace NotificationServiceFunction.Models.Enums
{
    public enum NotificationStatusEnum
    {
        [Description("Pending")]
        Pending = 0,
        [Description("Sent")]
        Sent = 1,
        [Description("Failed")]
        Failed = 2,
        [Description("Delivered")]
        Delivered = 3,
        [Description("Cancelled")]
        Cancelled = 4
    }
}
