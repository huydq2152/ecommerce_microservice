using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interface;
using Customer.API.Services;
using Customer.API.Services.Interface;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configuration;

namespace Customer.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        services.AddSingleton(databaseSettings);

        return services;
    }
    
    public static void ConfigureCustomerContext(this IServiceCollection services)
    {
        var databaseSettings = services.GetOption<DatabaseSettings>(nameof(DatabaseSettings));
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
        {
            throw new Exception("Connection string is not configured.");
        }

        services.AddDbContext<CustomerContext>(builder => builder.UseNpgsql(databaseSettings.ConnectionString));
    }
    
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<ICustomerService, CustomerService>();
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOption<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString,
                name: "PostgresQL Health",
                failureStatus: HealthStatus.Degraded);
    }
}