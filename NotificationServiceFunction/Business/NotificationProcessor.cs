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

        /// <summary>
        /// Azure Function triggered by a message in the <c>notificationsqueue</c>.
        /// Processes the incoming <see cref="NotificationQueueMessage"/> using the notification service,
        /// and logs success or validation failure messages accordingly.
        /// </summary>
        /// <param name="queueItem">The message received from the queue, containing notification data to process.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// If the message is valid and processed successfully, it logs a success message.
        /// If validation fails, a warning is logged and the message may be enqueued in a rejected queue by the service.
        /// If an unhandled exception occurs during processing, it is logged as an error and rethrown,
        /// which causes Azure Functions to move the message to the poison queue after retry exhaustion.
        /// </remarks>
        /// <exception cref="Exception">
        /// Any unhandled exception during processing is logged and rethrown to ensure Azure Functions
        /// retries the message or places it into the poison queue.
        /// </exception>

        [Function(nameof(NotificationProcessor))]
        public async Task Run([QueueTrigger("notificationsqueue", Connection = "AzureWebJobsStorage")] NotificationQueueMessage queueItem)
        {
            _logger.LogInformation($"Received message: {queueItem.ToString()}");

            try
            {
                var result = await _notificationService.ProcessAsync(queueItem);
                if (result.IsValid)
                    _logger.LogInformation($"Notification has been succesfully processed. '{queueItem.ToString()}'");
                else
                    _logger.LogWarning(result.ErrorMessage);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Processing failed: {ex.Message}");
                throw; //Re throw exception to insert message into poison queue.
            }
        }
    }
}
