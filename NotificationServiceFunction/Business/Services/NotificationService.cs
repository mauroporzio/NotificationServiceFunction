using Microsoft.Extensions.Logging;
using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ITableStorageService _storage;

        public NotificationService(ILogger<NotificationService>  logger, ITableStorageService storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public async Task<bool> ProcessAsync(NotificationQueueMessage queueMessage)
        {
            var limitInfo = NotificationRateLimits.Limits[queueMessage.NotificationType];
            var cutoffTime = queueMessage.TimestampUtc - limitInfo.Period;

            var recent = await _storage.GetRecentEventsAsync(queueMessage.Recipient, queueMessage.NotificationType, cutoffTime);

            if (recent is not null && recent.Count() >= limitInfo.Limit)
            {
                _logger.LogError($"Rate limit exceeded for {queueMessage.Recipient} - {queueMessage.NotificationType}");
                return false;
            }

            var ev = new NotificationEvent
            {
                PartitionKey = queueMessage.Recipient,
                RowKey = Guid.NewGuid().ToString(),
                NotificationType = queueMessage.NotificationType,
                TimestampUtc = queueMessage.TimestampUtc,
            };

            _logger.LogInformation($"Notification sent to {queueMessage.Recipient}: {queueMessage.Content}");

            await _storage.StoreEventAsync(ev);

            return true;
        }
    }
}
