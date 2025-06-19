using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface IRateLimitiBlobService
    {
        Task<List<NotificationRateLimit>?> GetRulesAsync();
    }
}
