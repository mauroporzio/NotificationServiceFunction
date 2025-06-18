using Azure;
using Azure.Data.Tables;

namespace NotificationServiceFunction.Models
{
    public class NotificationEvent : ITableEntity
    {
        public string PartitionKey { get; set; } // recipient
        public string RowKey { get; set; }  // unique ID
        public DateTime TimestampUtc { get; set; }
        public string NotificationType { get; set; }

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
