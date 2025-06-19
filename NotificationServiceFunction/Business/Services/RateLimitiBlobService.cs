using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models;
using NotificationServiceFunction.Models.Config;
using System.Data;
using System.Text.Json;

namespace NotificationServiceFunction.Business.Services
{
    public class RateLimitiBlobService : IRateLimitiBlobService
    {
        private readonly BlobClient _blobClient;

        public RateLimitiBlobService(IOptions<BlobStorageSettings> options)
        {
            var blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
            _blobClient = containerClient.GetBlobClient(options.Value.BlobName);
        }

        public async Task<List<NotificationRateLimit>?> GetRulesAsync()
        {
            var download = await _blobClient.DownloadContentAsync();
            var json = download.Value.Content.ToString();
            return JsonSerializer.Deserialize<List<NotificationRateLimit>>(json);
        }
    }
}
