using System.ComponentModel;

namespace NotificationServiceFunction.Models.Enums
{
    public enum NotificationTypesEnum
    {
        [Description("Status")]
        Status = 0,
        [Description("News")]
        News = 1,
        [Description("Marketing")]
        Marketing = 2,
    }
}
