using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationServiceFunction.Business.Services;
using NotificationServiceFunction.Business.Services.Interfaces;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITableStorageService, TableStorageService>();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
