using Microsoft.Extensions.Logging;
using Moq;
using NotificationServiceFunction.Business.Extensions;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models;
using NotificationServiceFunction.Models.Enums;

namespace NotificationServiceFunction.UnitTests.Tests.ServiceTests
{
    public class NotificationSeviceTests
    {
        public class NotificationServiceTests
        {
            #region Fixture

            private readonly Mock<ILogger<NotificationService>> _loggerMock = new();
            private readonly Mock<ITableStorageService> _storageMock = new();
            private readonly Mock<IRateLimitiBlobService> _blobMock = new();
            private readonly Mock<IRejectedNotificationQueueService> _rejectedQueueMock = new();

            private NotificationService CreateService()
            {
                return new NotificationService(
                    _loggerMock.Object,
                    _storageMock.Object,
                    _blobMock.Object,
                    _rejectedQueueMock.Object);
            }

            private NotificationQueueMessage CreateValidQueueMessage()
            {
                return new NotificationQueueMessage
                {
                    Recipient = "user@example.com",
                    NotificationType = NotificationTypesEnum.Marketing.GetDescription(),
                    Content = "Test content"
                };
            }

            private List<NotificationRateLimit> CreateRateLimits()
            {
                return new List<NotificationRateLimit>
                {
                    new NotificationRateLimit
                    {
                        NotificationType = NotificationTypesEnum.Marketing.GetDescription(),
                        RateLimit = 3,
                        TimeType = "Minutes",
                        TimeAmount = 10
                    }
                };
            }

            #endregion

            #region ProcessAsync

            [Fact]
            public async Task ProcessAsync_ValidMessage_StoresEvent_ReturnsValid()
            {
                // Arrange
                var service = CreateService();
                var message = CreateValidQueueMessage();
                var rateLimits = CreateRateLimits();

                _blobMock.Setup(b => b.GetRulesAsync())
                    .ReturnsAsync(rateLimits);

                _storageMock.Setup(s => s.GetRecentEventsAsync(message.Recipient, message.NotificationType, It.IsAny<DateTime>()))
                    .ReturnsAsync(new List<NotificationEvent>()); // No recent events

                _storageMock.Setup(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()))
                    .Returns(Task.CompletedTask);

                // Act
                var result = await service.ProcessAsync(message);

                // Assert
                Assert.True(result.IsValid);
                Assert.Null(result.ErrorMessage);

                // Verify that no downstream processing happened
                _storageMock.Verify(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()), Times.Once);
                _rejectedQueueMock.Verify(q => q.Enqueue(It.IsAny<NotificationQueueMessage>(), It.IsAny<string?>()), Times.Never);
            }

            [Fact]
            public async Task ProcessAsync_RateLimitExceeded_ReturnsInvalid_EnqueuesRejected()
            {
                // Arrange
                var service = CreateService();
                var message = CreateValidQueueMessage();
                var rateLimits = CreateRateLimits();

                var recentEvents = new List<NotificationEvent>();

                var recentEvent = new NotificationEvent
                {
                    PartitionKey = message.Recipient,
                    RowKey = Guid.NewGuid().ToString(),
                    NotificationType = message.NotificationType,
                    Timestamp = DateTime.Now,
                    Content = message.Content,
                    Status = (int)NotificationStatusEnum.Pending,
                    StatusDescription = NotificationStatusEnum.Pending.GetDescription()
                };

                //Add multiple times to simulate already sent similar messages within the time frame.
                recentEvents.Add(recentEvent);
                recentEvents.Add(recentEvent);
                recentEvents.Add(recentEvent);

                _blobMock.Setup(b => b.GetRulesAsync())
                    .ReturnsAsync(rateLimits);

                _storageMock.Setup(s => s.GetRecentEventsAsync(message.Recipient, message.NotificationType, It.IsAny<DateTime>()))
                    .ReturnsAsync(recentEvents);

                // Act
                var result = await service.ProcessAsync(message);

                // Assert
                Assert.False(result.IsValid);
                Assert.Contains("Rate limit exceeded", result.ErrorMessage);

                // Verify that no downstream processing happened
                _storageMock.Verify(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()), Times.Never);
                _rejectedQueueMock.Verify(q => q.Enqueue(message, result.ErrorMessage), Times.Once);
            }

            [Fact]
            public async Task ProcessAsync_InvalidNotificationType_ReturnsInvalid_EnqueuesRejected()
            {
                // Arrange
                var service = CreateService();
                var message = CreateValidQueueMessage();
                message.NotificationType = "OtherType"; //Invalid

                var rateLimits = CreateRateLimits();

                _blobMock.Setup(b => b.GetRulesAsync())
                    .ReturnsAsync(rateLimits);

                var expectedErrorMessage = $"No matching enum value found for description";

                // Act
                var result = await service.ProcessAsync(message);

                // Assert
                Assert.False(result.IsValid);
                Assert.NotNull(result.ErrorMessage);
                Assert.Contains(expectedErrorMessage, result.ErrorMessage);

                // Verify that no downstream processing happened
                _storageMock.Verify(s => s.GetRecentEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
                _storageMock.Verify(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()), Times.Never);

                //Asserts that the message has been sent to Rejected queue since it failed a business validation.
                _rejectedQueueMock.Verify(q => q.Enqueue(message, result.ErrorMessage), Times.Once);
            }

            [Fact]
            public async Task ProcessAsync_NoMatchingRateLimit_ReturnsInvalid_EnqueuesRejected()
            {
                // Arrange
                var service = CreateService();
                var message = CreateValidQueueMessage();

                // Provide rate limits missing the message.NotificationType
                var rateLimits = new List<NotificationRateLimit>
                {
                    new NotificationRateLimit
                    {
                        NotificationType = "OtherType", //Invalid
                        RateLimit = 1,
                        TimeType = "Minutes",
                        TimeAmount = 1
                    }
                };

                var expectedErrorMessage = $"No matching timeSpan configuration found for notifitcation type";

                _blobMock.Setup(b => b.GetRulesAsync())
                    .ReturnsAsync(rateLimits);

                // Act
                var result = await service.ProcessAsync(message);

                // Assert
                Assert.False(result.IsValid);
                Assert.NotNull(result.ErrorMessage);
                Assert.Contains(expectedErrorMessage, result.ErrorMessage);

                // Verify that no downstream processing happened
                _storageMock.Verify(s => s.GetRecentEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
                _storageMock.Verify(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()), Times.Never);

                //Asserts that the message has been sent to Rejected queue since it failed a business validation.
                _rejectedQueueMock.Verify(q => q.Enqueue(message, result.ErrorMessage), Times.Once);
            }

            [Fact]
            public async Task ProcessAsync_InvalidTimeSpan_ReturnsInvalid_EnqueuesRejected()
            {
                // Arrange
                var service = CreateService();
                var message = CreateValidQueueMessage();

                // Provide rate limits missing the message.NotificationType
                var rateLimits = new List<NotificationRateLimit>
                {
                    new NotificationRateLimit
                    {
                        NotificationType = message.NotificationType,
                        RateLimit = 1,
                        TimeType = "OtherTimeType", //Invalid
                        TimeAmount = 1
                    }
                }; 

                var expectedErrorMessage = $"No matching TimeSpanType found for";

                _blobMock.Setup(b => b.GetRulesAsync())
                    .ReturnsAsync(rateLimits);

                // Act
                var result = await service.ProcessAsync(message);

                // Assert
                Assert.False(result.IsValid);
                Assert.NotNull(result.ErrorMessage);
                Assert.Contains(expectedErrorMessage, result.ErrorMessage);

                // Verify that no downstream processing happened
                _storageMock.Verify(s => s.GetRecentEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
                _storageMock.Verify(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()), Times.Never);

                //Asserts that the message has been sent to Rejected queue since it failed a business validation.
                _rejectedQueueMock.Verify(q => q.Enqueue(message, result.ErrorMessage), Times.Once);
            }

            [Fact]
            public async Task ProcessAsync_ThrowsException_DoesNotEnqueueRejected()
            {
                // Arrange
                var service = CreateService();
                var message = CreateValidQueueMessage();

                _blobMock.Setup(b => b.GetRulesAsync())
                    .ThrowsAsync(new Exception("Fatal Error"));

                // Act & Assert
                var ex = await Assert.ThrowsAsync<Exception>(() => service.ProcessAsync(message));

                // Verify that no downstream processing happened
                _storageMock.Verify(s => s.GetRecentEventsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
                _storageMock.Verify(s => s.StoreEventAsync(It.IsAny<NotificationEvent>()), Times.Never);

                // Verify that the rejected queue was NOT used for technical errors
                _rejectedQueueMock.Verify(q => q.Enqueue(It.IsAny<NotificationQueueMessage>(), It.IsAny<string>()), Times.Never);
            }

            #endregion
        }
    }
}
