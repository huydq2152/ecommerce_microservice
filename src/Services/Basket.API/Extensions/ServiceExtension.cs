using Basket.API.Repositories;
using Basket.API.Repositories.Interface;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

namespace Basket.API.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>();
        return services;
    }

    public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new ArgumentNullException("Redis connnection string is not configured");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });
    }
}