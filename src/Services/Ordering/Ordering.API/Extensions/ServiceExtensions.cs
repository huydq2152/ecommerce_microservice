using Contracts.Configurations;
using EventBus.Messages.IntegrationEvents.Events;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.API.Application.IntegrationEvents.EventsHandler;
using Shared.Configuration;

namespace Ordering.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSetting = configuration.GetSection(nameof(SmtpEmailSetting)).Get<SmtpEmailSetting>();
        if (emailSetting == null) throw new ArgumentNullException("smtp email setting is not configured");
        services.AddSingleton(emailSetting);

        var eventBusSetting = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>();
        if (eventBusSetting == null) throw new ArgumentNullException("event bus setting is not configured");
        services.AddSingleton(eventBusSetting);
        
        return services;
    }

    public static void ConfigureMasstransit(this IServiceCollection services)
    {
        var eventBusSetting = services.GetOption<EventBusSettings>(nameof(EventBusSettings));
        if (eventBusSetting == null || string.IsNullOrEmpty(eventBusSetting.HostAddress))
        {
            throw new ArgumentNullException("Event bus settings is not configured");
        }
        
        var mqConnection = new Uri(eventBusSetting.HostAddress);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumersFromNamespaceContaining<BasketCheckoutEventHandler>();
            configurator.UsingRabbitMq(((context, factoryConfigurator) =>
            {
                factoryConfigurator.Host(mqConnection);
                factoryConfigurator.ConfigureEndpoints(context);
            }));
        });
    }
}