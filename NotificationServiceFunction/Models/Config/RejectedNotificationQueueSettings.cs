namespace NotificationServiceFunction.Models.Config
{
    public class RejectedNotificationQueueSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
