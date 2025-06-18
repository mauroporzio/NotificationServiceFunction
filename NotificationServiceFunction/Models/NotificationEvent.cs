using Azure;
using Azure.Data.Tables;

namespace NotificationServiceFunction.Models
{
    public class NotificationEvent : ITableEntity
    {
        public required string PartitionKey { get; set; } // recipient
        public required string RowKey { get; set; }  // unique ID
        public DateTime TimestampUtc { get; set; }
        public required string NotificationType { get; set; }

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
