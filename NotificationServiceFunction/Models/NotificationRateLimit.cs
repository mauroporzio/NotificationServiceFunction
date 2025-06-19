namespace NotificationServiceFunction.Models
{
    public class NotificationRateLimit
    {
        public required string NotificationType { get; set; }
        public required int RateLimit { get; set; }
        public required string TimeType { get; set; }
    }
}
