using Azure;
using Azure.Data.Tables;

namespace NotificationServiceFunction.Models
{
    public class NotificationEvent : ITableEntity
    {
        public required string PartitionKey { get; set; } // recipient
        public required string RowKey { get; set; }  // unique ID
        public required string NotificationType { get; set; }
        public required string Content { get; set; }
        public required int Status { get; set; }
        public required string StatusDescription { get; set; }

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public override string ToString()
        {
            return $"PartitionKey: '{this.PartitionKey}', RowKey: '{this.RowKey}', NotificationType: '{this.NotificationType}', Content: '{this.Content}', Status: {this.Status}, ETag: {this.ETag}, Status: {this.Timestamp}";
        }
    }
}
