using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NotificationServiceFunction.Business.Services.Interfaces;

namespace NotificationServiceFunction.Business
{
    public class NotificationProcessor
    {
        private readonly ILogger<NotificationProcessor> _logger;
        private readonly NotificationService _notificationService;

        public NotificationProcessor(ILogger<NotificationProcessor> logger)
        {
            _logger = logger;
            _notificationService = new NotificationService();
        }

        [Function(nameof(NotificationProcessor))]
        public async Task Run([QueueTrigger("notificationsQueue", Connection = "StorageConnection")] QueueMessage queueItem)
        {
            _logger.LogInformation($"Received message: {queueItem}");

            try
            {
                var success = await _notificationService.ProcessAsync(queueItem);
                if (!success)
                {
                    _logger.LogWarning("Notification dropped due to rate limiting.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Processing failed: {ex.Message}");
            }
        }
    }
}
