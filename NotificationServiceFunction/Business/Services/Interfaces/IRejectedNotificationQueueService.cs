using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface IRejectedNotificationQueueService
    {
        /// <summary>
        /// Enqueues a rejected notification message into the RejectedNotifications queue,
        /// including the reason for rejection.
        /// </summary>
        /// <param name="notificationQueueMessage">The original notification message that failed validation or processing.</param>
        /// <param name="reason">The reason why the message was rejected. This value may be null.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// The method creates a <see cref="RejectedNotification"/> object based on the provided message and reason,
        /// serializes it to JSON, and enqueues it using the configured <c>_queueClient</c>.
        /// A log entry is written once the message is sent.
        /// </remarks>
        /// <exception cref="Azure.RequestFailedException">
        /// Thrown if the queue message fails to send due to storage issues such as connectivity or authorization errors.
        /// </exception>
        
        Task Enqueue(NotificationQueueMessage notificationQueueMessage, string? reason);
    }
}
