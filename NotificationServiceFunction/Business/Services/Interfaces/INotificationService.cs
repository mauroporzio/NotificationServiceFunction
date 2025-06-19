using Azure.Storage.Queues.Models;
using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface INotificationService
    {
        Task<(bool IsValid, string? ErrorMessage)> ProcessAsync(NotificationQueueMessage queueMessage);
    }
}
