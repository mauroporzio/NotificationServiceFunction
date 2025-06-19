using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationServiceFunction.Business.Services;
using NotificationServiceFunction.Business.Services.Interfaces;
using NotificationServiceFunction.Models.Config;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITableStorageService, TableStorageService>();
builder.Services.AddScoped<IRateLimitiBlobService, RateLimitiBlobService>();

builder.Services.Configure<TableStorageSettings>(
    builder.Configuration.GetSection("NotificationEventsTable"));

builder.Services.Configure<BlobStorageSettings>(
    builder.Configuration.GetSection("NotificationRateLimitsBlobStorage"));

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
