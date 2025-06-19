using NotificationServiceFunction.Models;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public interface ITableStorageService
    {
        /// <summary>
        /// Retrieves recent pending notification events for a specific recipient and notification type,
        /// filtered by a cutoff timestamp.
        /// </summary>
        /// <param name="recipient">The recipient's identifier, used as the partition key in the table.</param>
        /// <param name="type">The notification type to filter by (must match the <c>NotificationType</c> property).</param>
        /// <param name="cutoffTime">The earliest timestamp to include; events older than this are excluded.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of
        /// <see cref="NotificationEvent"/> instances that match the filter criteria.
        /// </returns>
        /// <remarks>
        /// This method queries the underlying table storage using the provided filters:
        /// recipient (partition key), notification type, timestamp greater than or equal to <paramref name="cutoffTime"/>,
        /// and status equal to <c>Pending</c>.
        /// </remarks>
        Task<IEnumerable<NotificationEvent>> GetRecentEventsAsync(string recipient, string type, DateTime cutoffTime);

        /// <summary>
        /// Stores a <see cref="NotificationEvent"/> entity in the Azure Table Storage.
        /// </summary>
        /// <param name="notificationEvent">The event to store, including details such as recipient, type, timestamp, and content.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method adds the provided <paramref name="notificationEvent"/> to the configured table using <c>AddEntityAsync</c>.
        /// </remarks>
        /// <exception cref="Azure.RequestFailedException">
        /// Thrown if the operation fails due to storage-related issues such as connectivity, conflicts, or permissions.
        /// </exception>

        Task StoreEventAsync(NotificationEvent notificationEvent);
    }
}
