namespace NotificationServiceFunction.Models
{
    public class RejectedNotification
    {
        public required string Recipient { get; set; }
        public required string NotificationType { get; set; }
        public required string Content { get; set; }
        public required string? Reason { get; set; }

        public override string ToString()
        {
            return $"Recipient: '{this.Recipient}', NotificationType: '{this.NotificationType}', Content: '{this.Content}', Reason: {this.Reason}";
        }
    }
}
