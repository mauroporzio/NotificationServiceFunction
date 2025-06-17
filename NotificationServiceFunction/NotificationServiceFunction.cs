using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace NotificationServiceFunction
{
    public class NotificationServiceFunction
    {
        private readonly ILogger<NotificationServiceFunction> _logger;

        public NotificationServiceFunction(ILogger<NotificationServiceFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(NotificationServiceFunction))]
        public void Run([QueueTrigger("notifcationsQueue", Connection = "StorageConnection")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
