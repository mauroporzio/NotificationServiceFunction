using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface IRateLimitiBlobService
    {
        /// <summary>
        /// Downloads and deserializes the notification rate limit rules from the configured blob storage.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of <see cref="NotificationRateLimit"/> instances.
        /// </returns>
        /// <exception cref="Azure.RequestFailedException">
        /// Thrown if the blob download operation fails due to storage connectivity or access issues.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the blob content cannot be deserialized into a list of <see cref="NotificationRateLimit"/>.
        /// </exception>
        /// <remarks>
        /// This method retrieves the blob content as a string and attempts to deserialize it into a list of <see cref="NotificationRateLimit"/>.
        /// If deserialization fails, an <see cref="InvalidOperationException"/> is thrown.
        /// </remarks>
        
        Task<List<NotificationRateLimit>> GetRulesAsync();
    }
}
