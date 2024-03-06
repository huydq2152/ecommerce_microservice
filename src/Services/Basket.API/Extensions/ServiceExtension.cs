using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interface;
using Basket.API.Services;
using Basket.API.Services.Interface;
using Common.Logging;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Interface;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using Inventory.Grpc.Client;
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

        var grpcSettings = configuration.GetSection(nameof(GrpcSettings)).Get<GrpcSettings>();
        if (grpcSettings == null) throw new ArgumentNullException("Grpc settings is not configured");
        services.AddSingleton(grpcSettings);

        var backgroundJobSettings =
            configuration.GetSection(nameof(BackgroundJobSettings)).Get<BackgroundJobSettings>();
        if (backgroundJobSettings == null) throw new ArgumentNullException("BackgroundJobSettings is not configured");
        services.AddSingleton(backgroundJobSettings);

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>()
            .AddTransient<IEmailTemplateService, BasketEmailTemplateService>()
            .AddTransient<LoggingDelegatingHandler>();
        return services;
    }

    public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var cacheSetting = services.GetOption<CacheSettings>(nameof(CacheSettings));
        if (string.IsNullOrEmpty(cacheSetting.ConnectionString))
        {
            throw new ArgumentNullException("Redis connnection string is not configured");
        }

        services.AddStackExchangeRedisCache(options => { options.Configuration = cacheSetting.ConnectionString; });
    }

    public static IServiceCollection ConfigureGrpcServices(this IServiceCollection services)
    {
        var settings = services.GetOption<GrpcSettings>(nameof(GrpcSettings));
        services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(o => o.Address = new Uri(settings.StockUrl));
        services.AddScoped<StockItemGrpcService>();

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
            configurator.UsingRabbitMq((context, factoryConfigurator) => { factoryConfigurator.Host(mqConnection); });

            //public submit order message
            configurator.AddRequestClient<IBasketCheckoutEvent>();
        });
    }

    public static void ConfigureHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient<BackgroundJobHttpService>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseImmediateRetryPolicy();
    }
}