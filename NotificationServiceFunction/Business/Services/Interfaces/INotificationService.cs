using Azure.Storage.Queues.Models;
using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Processes a notification queue message by validating it against rate-limiting rules,
        /// storing it if allowed, or enqueueing it in a rejected queue if it violates rules.
        /// </summary>
        /// <param name="queueMessage">The <see cref="NotificationQueueMessage"/> to process, containing recipient and notification details.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><term><c>IsValid</c></term><description>True if the notification passed validation and was stored; otherwise, false.</description></item>
        /// <item><term><c>ErrorMessage</c></term><description>Null if valid; otherwise, a string describing the validation failure reason.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The method retrieves rate-limiting rules from blob storage and applies them based on the message's notification type.
        /// If the rate limit is exceeded, the message is not stored and is sent to the rejected queue.
        /// </remarks>
        /// <exception cref="Exception">Throws if underlying blob or table storage operations fail unexpectedly.</exception>

        Task<(bool IsValid, string? ErrorMessage)> ProcessAsync(NotificationQueueMessage queueMessage);
    }
}
