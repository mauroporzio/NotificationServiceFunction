using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business
{
    public class NotificationProcessor
    {
        private readonly ILogger<NotificationProcessor> _logger;
        private readonly INotificationService _notificationService;

        public NotificationProcessor(ILogger<NotificationProcessor> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [Function(nameof(NotificationProcessor))]
        public async Task Run([QueueTrigger("notificationsqueue", Connection = "StorageConnection")] NotificationQueueMessage queueItem)
        {
            _logger.LogInformation($"Received message: {queueItem}");

            try
            {
                if (!await _notificationService.ProcessAsync(queueItem))
                    _logger.LogWarning("Notification dropped due to rate limiting.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Processing failed: {ex.Message}");
            }
        }
    }
}
