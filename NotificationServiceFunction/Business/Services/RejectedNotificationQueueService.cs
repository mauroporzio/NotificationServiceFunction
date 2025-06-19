using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models;
using NotificationServiceFunction.Models.Config;
using System.Text.Json;

namespace NotificationServiceFunction.Business.Services
{
    public class RejectedNotificationQueueService : IRejectedNotificationQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<RejectedNotificationQueueService> _logger;

        public RejectedNotificationQueueService(IOptions<RejectedNotificationQueueSettings> options, ILogger<RejectedNotificationQueueService> logger)
        {
            var connectionString = options.Value.ConnectionString;
            var queueName = options.Value.QueueName;

            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
            _logger = logger;
        }

        public async Task Enqueue(NotificationQueueMessage notificationQueueMessage, string? reason)
        {
            var message = new RejectedNotification()
            {
                Recipient = notificationQueueMessage.Recipient,
                Content = notificationQueueMessage.Content,
                NotificationType = notificationQueueMessage.NotificationType,
                Reason = reason
            };

            string messageText = JsonSerializer.Serialize(message);

            await _queueClient.SendMessageAsync(messageText);

            _logger.LogInformation($"Sent message to RejectedNotifications queue: '{message.ToString()}'");
        }
    }
}
