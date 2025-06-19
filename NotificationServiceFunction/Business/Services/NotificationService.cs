using Microsoft.Extensions.Logging;
using NotificationServiceFunction.Business.Extensions;
using NotificationServiceFunction.Business.Helper;
using NotificationServiceFunction.Models;
using NotificationServiceFunction.Models.Constants;
using NotificationServiceFunction.Models.Enums;

namespace NotificationServiceFunction.Business.Services.Interfaces
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ITableStorageService _storage;
        private readonly IRateLimitiBlobService _blob;
        private readonly IRejectedNotificationQueueService _rejectedNotificationQueueService;

        public NotificationService(ILogger<NotificationService>  logger, ITableStorageService storage, IRateLimitiBlobService blob, IRejectedNotificationQueueService rejectedNotificationQueueService)
        {
            _logger = logger;
            _storage = storage;
            _blob = blob;
            _rejectedNotificationQueueService = rejectedNotificationQueueService;
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ProcessAsync(NotificationQueueMessage queueMessage)
        {
            (bool IsValid, string? ErrorMessage) result = (true, null);

            var notificationsRateLimits = await _blob.GetRulesAsync();

            var validationResult = ValidateQueueMessage(queueMessage, notificationsRateLimits);

            if(validationResult.IsValid)
            {
                var notificationType = EnumExtensions.FromDescription<NotificationTypesEnum>(queueMessage.NotificationType);
                var limitInfo = GetNotificationRateLimit(notificationsRateLimits, notificationType.GetDescription());

                var currentTime = DateTime.UtcNow;
                var timeSpan = TimeSpanHelper.GetTimeSpan(limitInfo.TimeType, limitInfo.TimeAmount);
                var cutoffTime = currentTime - timeSpan;

                var recent = await _storage.GetRecentEventsAsync(queueMessage.Recipient, notificationType.GetDescription(), cutoffTime);

                if (recent != null && recent.Count() >= limitInfo.RateLimit)
                {
                    result = (false, $"Rate limit exceeded for {queueMessage.Recipient} - {notificationType.GetDescription()}");
                } 
                else
                {
                    var ev = new NotificationEvent
                    {
                        PartitionKey = queueMessage.Recipient,
                        RowKey = Guid.NewGuid().ToString(),
                        NotificationType = notificationType.GetDescription(),
                        Timestamp = currentTime,
                        Content = queueMessage.Content,
                        Status = (int)NotificationStatusEnum.Pending
                    };

                    await _storage.StoreEventAsync(ev);

                    _logger.LogInformation($"Notification inserted into NotificationEvents table: '{ev.ToString()}' ");

                    result = (true, null);
                }
            }
            else
            {
                result = (false, validationResult.ErrorMessage);
            }
                
            if(!result.IsValid)
                await _rejectedNotificationQueueService.Enqueue(queueMessage, result.ErrorMessage);

            return result;
        }

        /// <summary>
        /// Validates the contents of a <see cref="NotificationQueueMessage"/> against supported notification types
        /// and rate limit configuration rules.
        /// </summary>
        /// <param name="queueMessage">The message to validate, containing type and recipient information.</param>
        /// <param name="notificationsRateLimits">The list of configured rate limits used for validation. May be null.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><term><c>IsValid</c></term><description>True if the message passed all validations; otherwise, false.</description></item>
        /// <item><term><c>ErrorMessage</c></term><description>An error message describing the validation failure, or empty if valid.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// The method performs three validations:
        /// <list type="number">
        /// <item>That the notification type is recognized by the <see cref="NotificationTypesEnum"/>.</item>
        /// <item>That a rate limit rule exists for the given notification type.</item>
        /// <item>That the time span type in the rule is supported by the system.</item>
        /// </list>
        /// If any of these checks fail, the method returns <c>IsValid = false</c> with an appropriate <c>ErrorMessage</c>.
        /// </remarks>

        private (bool IsValid, string ErrorMessage) ValidateQueueMessage(NotificationQueueMessage queueMessage, List<NotificationRateLimit>? notificationsRateLimits)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            try
            {
                //Validate that NotificationType exists and is supported.
                EnumExtensions.FromDescription<NotificationTypesEnum>(queueMessage.NotificationType);

                //Validate that the time rate configuration for the notification type exists.
                var timeSpan = GetNotificationRateLimit(notificationsRateLimits, queueMessage.NotificationType);

                //Validate that timeSpan exists and is supported.
                TimeSpanTypesConstants.IsValidTimeSpanType(timeSpan.TimeType);
            }
            catch (Exception ex)
            {
                isValid = false;
                errorMessage = ex.Message;
            }

            return (isValid, errorMessage);
        }

        /// <summary>
        /// Retrieves the rate limit configuration for a given notification type from the provided list of rate limits.
        /// </summary>
        /// <param name="notificationsRateLimits">The list of available rate limit rules. May be null.</param>
        /// <param name="notificationType">The notification type to search for in the rate limit configuration.</param>
        /// <returns>
        /// The <see cref="NotificationRateLimit"/> that matches the given notification type.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when no matching configuration is found for the specified <paramref name="notificationType"/>.
        /// </exception>
        /// <remarks>
        /// This method uses <c>FirstOrDefault</c> to locate a matching <see cref="NotificationRateLimit"/> based on
        /// the <c>NotificationType</c> field. If no match is found, it throws an exception with a descriptive message.
        /// </remarks>

        private NotificationRateLimit GetNotificationRateLimit(List<NotificationRateLimit>? notificationsRateLimits, string notificationType)
        {
            return notificationsRateLimits?.FirstOrDefault(x => x.NotificationType == notificationType) ??
                   throw new Exception($"No matching timeSpan configuration found for notifitcation type: '{notificationType}'");
        }
    }
}
