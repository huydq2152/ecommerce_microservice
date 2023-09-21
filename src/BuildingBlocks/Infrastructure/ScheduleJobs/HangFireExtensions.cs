using System.Security.Authentication;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Shared.Configuration;

namespace Infrastructure.ScheduleJobs;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireService(this IServiceCollection services)
    {
        var settings = services.GetOption<HangfireSettings>(nameof(HangfireSettings));
        if (settings == null || settings.Storage == null || string.IsNullOrEmpty(settings.Storage.ConnectionString))
        {
            throw new Exception("Hangfire setting is not configured properly");
        }

        services.ConfigureHangfireService(settings);
        services.AddHangfireServer(provider => { provider.ServerName = settings.ServerName; });

        return services;
    }

    private static IServiceCollection ConfigureHangfireService(this IServiceCollection services,
        HangfireSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Storage.DBProvider))
        {
            throw new Exception("Hangfire DbProvider is not configured");
        }

        switch (settings.Storage.DBProvider.ToLower())
        {
            case "mongodb":
                var mongoUrlBuilder = new MongoUrlBuilder(settings.Storage.ConnectionString);
                var mongoClientSetting = MongoClientSettings.FromUrl(new MongoUrl(settings.Storage.ConnectionString));
                mongoClientSetting.SslSettings = new SslSettings()
                {
                    EnabledSslProtocols = SslProtocols.Tls12
                };
                var mongoClient = new MongoClient(mongoClientSetting);
                var mongoStorageOption = new MongoStorageOptions()
                {
                    MigrationOptions = new MongoMigrationOptions()
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    CheckConnection = true,
                    Prefix = "SchedulerQueue",
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                };
                services.AddHangfire((provider, configuration) =>
                {
                    configuration.UseSimpleAssemblyNameTypeSerializer()
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseRecommendedSerializerSettings()
                        .UseConsole()
                        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, mongoStorageOption);
                    var jsonSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    configuration.UseSerializerSettings(jsonSettings);
                });
                services.AddHangfireConsoleExtensions();
                break;
            case "postgresql":
                break;
            case "mssql":
                break;
            default:
                throw new Exception("Hangfire Storage ");
                break;
        }

        return services;
    }
}