using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationServiceFunction.Business.Services;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models.Config;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITableStorageService, TableStorageService>();
builder.Services.AddScoped<IRateLimitiBlobService, RateLimitiBlobService>();
builder.Services.AddScoped<IRejectedNotificationQueueService, RejectedNotificationQueueService>();

builder.Services.Configure<TableStorageSettings>(
    builder.Configuration.GetSection("NotificationEventsTable"));

builder.Services.Configure<BlobStorageSettings>(
    builder.Configuration.GetSection("NotificationRateLimitsBlobStorage"));

builder.Services.Configure<RejectedNotificationQueueSettings>(
    builder.Configuration.GetSection("RejectedNotificationQueue"));

builder.Services.Configure<QueuesOptions>(options =>
{
    options.MaxPollingInterval = TimeSpan.FromSeconds(1);
    options.BatchSize = 1;
    options.MaxDequeueCount = 5;
    options.VisibilityTimeout = TimeSpan.FromMinutes(5);
});

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
