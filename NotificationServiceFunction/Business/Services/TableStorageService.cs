using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models;
using NotificationServiceFunction.Models.Config;
using NotificationServiceFunction.Models.Enums;

namespace NotificationServiceFunction.Business.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(IOptions<TableStorageSettings> options)
        {
            var connectionString = options.Value.ConnectionString;
            var tableName = options.Value.TableName;

            _tableClient = new TableClient(connectionString, tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task<IEnumerable<NotificationEvent>> GetRecentEventsAsync(string recipient, string type, DateTime cutoffTime)
        {
            var list = new List<NotificationEvent>();

            var filter = TableClient.CreateQueryFilter<NotificationEvent>(e =>
                e.PartitionKey == recipient &&
                e.NotificationType == type &&
                e.TimestampUtc >= cutoffTime &&
                e.Status == (int)NotificationStatusEnum.Pending);

            var result = _tableClient.QueryAsync<NotificationEvent>(filter);

            await foreach (var entity in result)
            {
                list.Add(entity);
            }

            return list;
        }

        public async Task StoreEventAsync(NotificationEvent notificationEvent)
        {
            await _tableClient.AddEntityAsync(notificationEvent);
        }
    }
}
