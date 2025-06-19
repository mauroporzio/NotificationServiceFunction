using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface IRejectedNotificationQueueService
    {
        Task Enqueue(NotificationQueueMessage notificationQueueMessage, string? reason);
    }
}
