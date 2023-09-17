using Basket.API.Repositories;
using Basket.API.Repositories.Interface;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Interface;
using Infrastructure.Common;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configuration;

namespace Basket.API.Extensions;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings)).Get<EventBusSettings>();
        if (eventBusSettings == null) throw new ArgumentNullException("Event bus settings is not configured");
        services.AddSingleton(eventBusSettings);
        
        var cacheSettings = configuration.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        if (cacheSettings == null) throw new ArgumentNullException("Cache settings is not configured");
        services.AddSingleton(cacheSettings);

        return services;
    }
    
    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>();
        return services;
    }

    public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheSetting = services.GetOption<CacheSettings>(nameof(CacheSettings));
        if (string.IsNullOrEmpty(cacheSetting.ConnectionString))
        {
            throw new ArgumentNullException("Redis connnection string is not configured");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheSetting.ConnectionString;
        });
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
            configurator.UsingRabbitMq((context, factoryConfigurator) =>
            {
               factoryConfigurator.Host(mqConnection);
            });
            
            //public submit order message
            configurator.AddRequestClient<IBasketCheckoutEvent>();
        });
    }
}