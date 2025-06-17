using Azure.Storage.Queues.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> ProcessAsync(QueueMessage queueMessage);
    }
}
