namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> ProcessAsync(string queueMessageJson);
    }
}
