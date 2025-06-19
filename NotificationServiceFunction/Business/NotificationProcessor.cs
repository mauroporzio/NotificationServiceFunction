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
        public async Task Run([QueueTrigger("notificationsqueue", Connection = "AzureWebJobsStorage")] NotificationQueueMessage queueItem)
        {
            _logger.LogInformation($"Received message: {queueItem.ToString()}");

            try
            {
                var result = await _notificationService.ProcessAsync(queueItem);
                if (!result.IsValid)
                    _logger.LogWarning(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Processing failed: {ex.Message}");
            }
        }
    }
}
