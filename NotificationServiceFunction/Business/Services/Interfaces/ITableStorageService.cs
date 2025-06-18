using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface ITableStorageService
    {
        Task<IEnumerable<NotificationEvent>> GetRecentEventsAsync(string recipient, string type, DateTime cutoffTime);
        Task StoreEventAsync(NotificationEvent notificationEvent);
    }
}
