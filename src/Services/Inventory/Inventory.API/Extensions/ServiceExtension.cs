using Infrastructure.Extensions;
using Inventory.API.Services;
using Inventory.API.Services.Interfaces;
using MongoDB.Driver;
using Shared.Configuration;

namespace Inventory.API.Extensions;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
        services.AddSingleton(databaseSettings);

        return services;
    }

    private static string GetMongoConnectionString(IServiceCollection services)
    {
        var settings = services.GetOption<MongoDbSettings>(nameof(MongoDbSettings));
        if (settings == null || string.IsNullOrEmpty(settings.ToString()))
        {
            throw new ArgumentNullException("MongoDbSettings is not configured");
        }

        var databaseName = settings.DatabaseName;
        var mongoDbConnectionString = $"{settings.ConnectionString}/{databaseName}?authSource=admin";
        return mongoDbConnectionString;
    }

    public static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(GetMongoConnectionString(services)))
            .AddScoped(o => o.GetService<IMongoClient>()?.StartSession());
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
        services.AddScoped<IInventoryService, InventoryService>();
    }
}