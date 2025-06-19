using Microsoft.Extensions.Logging;
using NotificationServiceFunction.Business.Extensions;
using NotificationServiceFunction.Business.Helper;
using NotificationServiceFunction.Models;
using NotificationServiceFunction.Models.Enums;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ITableStorageService _storage;
        private readonly IRateLimitiBlobService _blob;

        public NotificationService(ILogger<NotificationService>  logger, ITableStorageService storage, IRateLimitiBlobService blob)
        {
            _logger = logger;
            _storage = storage;
            _blob = blob;
        }

        public async Task<bool> ProcessAsync(NotificationQueueMessage queueMessage)
        {
            var currentTime = DateTime.UtcNow;
            var notificationType = EnumExtensions.FromDescription<NotificationTypesEnum>(queueMessage.NotificationType);
            var limitInfo = await GetNotificationRateLimit(notificationType.GetDescription());

            if(limitInfo != null)
            {
                var timeSpan = TimeSpanHelper.GetTimeSpan(limitInfo.TimeType, limitInfo.TimeAmount);
                var cutoffTime = currentTime - timeSpan;

                var recent = await _storage.GetRecentEventsAsync(queueMessage.Recipient, notificationType.GetDescription(), cutoffTime);

                if (recent != null && recent.Count() >= limitInfo.RateLimit)
                {
                    _logger.LogError($"Rate limit exceeded for {queueMessage.Recipient} - {notificationType.GetDescription()}");
                    return false;
                }

                var ev = new NotificationEvent
                {
                    PartitionKey = queueMessage.Recipient,
                    RowKey = Guid.NewGuid().ToString(),
                    NotificationType = notificationType.GetDescription(),
                    Timestamp = currentTime,
                    Content = queueMessage.Content,
                    Status = (int)NotificationStatusEnum.Pending
                };

                _logger.LogInformation($"Notification sent to {queueMessage.Recipient}: {queueMessage.Content}");

                await _storage.StoreEventAsync(ev);

                return true;
            }

            _logger.LogError($"Rate limit not found for {notificationType.GetDescription()}");
            return false;
        }

        private async Task<NotificationRateLimit?> GetNotificationRateLimit(string notificationType)
        {
            var notificationsRateLimitis = await _blob.GetRulesAsync();

            return notificationsRateLimitis?.FirstOrDefault(x => x.NotificationType == notificationType);
        }
    }
}
