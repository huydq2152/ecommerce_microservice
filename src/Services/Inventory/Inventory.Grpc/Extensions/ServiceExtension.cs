﻿using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Inventory.Grpc.Repositories;
using Inventory.Grpc.Repositories.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Shared.Configuration;

namespace Inventory.Grpc.Extensions;

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
        services.AddScoped<IInventoryRepository, InventoryRepository>();
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOption<MongoDbSettings>(nameof(MongoDbSettings));
        services.AddSingleton<HealthServiceImpl>();
        services.AddHostedService<StatusService>();
        services.AddHealthChecks()
            .AddMongoDb(databaseSettings.ConnectionString,
                name: "Inventory MongoDb Health",
                failureStatus: HealthStatus.Degraded)
            .AddCheck("Inventory Grpc Health", () => HealthCheckResult.Healthy());
    }
}