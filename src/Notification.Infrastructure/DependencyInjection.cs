using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Common.Messaging;
using Notification.Domain.Repositories;
using Notification.Domain.Services;
using Notification.Infrastructure.Messaging;
using Notification.Infrastructure.Persistence;
using Notification.Infrastructure.Services;
using Notification.Infrastructure.Services.Senders;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddMessaging(configuration);
        services.AddNotificationSenders();

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Use in-memory database for development/testing
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseInMemoryDatabase("NotificationDb"));
        }
        else
        {
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var messagingProvider = configuration.GetValue<string>("Messaging:Provider") ?? "InMemory";

        switch (messagingProvider.ToLowerInvariant())
        {
            case "rabbitmq":
                services.Configure<RabbitMqOptions>(
                    configuration.GetSection(RabbitMqOptions.SectionName));
                services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
                break;

            case "azureservicebus":
                services.Configure<AzureServiceBusOptions>(
                    configuration.GetSection(AzureServiceBusOptions.SectionName));
                services.AddSingleton<IMessagePublisher, AzureServiceBusMessagePublisher>();
                break;

            case "inmemory":
            default:
                // In-memory publisher for development/testing (logs messages only)
                services.AddSingleton<IMessagePublisher, InMemoryMessagePublisher>();
                break;
        }

        return services;
    }

    private static IServiceCollection AddNotificationSenders(this IServiceCollection services)
    {
        // Note: These senders are now primarily used by channel microservices.
        // The orchestrator API publishes to queues instead of using these directly.
        // Keeping them here for backward compatibility and potential direct-send scenarios.

        services.AddHttpClient("WebhookClient", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<INotificationSender, EmailNotificationSender>();
        services.AddScoped<INotificationSender, SmsNotificationSender>();
        services.AddScoped<INotificationSender, PushNotificationSender>();
        services.AddScoped<INotificationSender, WebhookNotificationSender>();
        services.AddScoped<INotificationSender, InAppNotificationSender>();

        services.AddScoped<INotificationSenderFactory, NotificationSenderFactory>();

        return services;
    }
}
