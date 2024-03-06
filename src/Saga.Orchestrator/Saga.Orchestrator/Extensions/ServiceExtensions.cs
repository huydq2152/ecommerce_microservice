using Common.Logging;
using Contracts.Saga.OrderManager;
using Infrastructure.Extensions;
using Infrastructure.Policies;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.Configuration;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICheckoutSagaService, CheckoutSagaService>()
            .AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>()
            .AddTransient<LoggingDelegatingHandler>();
        return services;
    }

    public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services)
    {
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
            .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
        return services;
    }
    
    public static void ConfigureHttpClient(this IServiceCollection services)
    {
        ConfigureOrderHttpClient(services);
        ConfigureBasketHttpClient(services);
        ConfigureInventoryHttpClient(services);
    }
    
    private static void ConfigureOrderHttpClient(IServiceCollection services)
    {
        var urls = services.GetOption<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Orders))
            throw new ArgumentNullException("ServiceUrls Orders is not configured");
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri($"{urls.Orders}/api/v1/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrdersAPI"));
    }
    
    private static void ConfigureBasketHttpClient(IServiceCollection services)
    {
        var urls = services.GetOption<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Basket))
            throw new ArgumentNullException("ServiceUrls Basket is not configured");
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri($"{urls.Basket}/api/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialRetryPolicy();
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketsAPI"));
    }
    
    private static void ConfigureInventoryHttpClient(IServiceCollection services)
    {
        var urls = services.GetOption<ServiceUrls>(nameof(ServiceUrls));
        if (urls == null || string.IsNullOrEmpty(urls.Inventory))
            throw new ArgumentNullException("ServiceUrls Inventory is not configured");
        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri($"{urls.Inventory}/api/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialRetryPolicy();;
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryAPI"));
    }
}