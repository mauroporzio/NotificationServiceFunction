using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NotificationServiceFunction.Business
{
    public class NotificationProcessor
    {
        private readonly ILogger<NotificationProcessor> _logger;

        public NotificationProcessor(ILogger<NotificationProcessor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(NotificationProcessor))]
        public void Run([QueueTrigger("notificationsQueue", Connection = "StorageConnection")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
