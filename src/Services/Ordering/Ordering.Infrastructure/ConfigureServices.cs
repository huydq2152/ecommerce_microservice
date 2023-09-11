using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrderContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
                builder => builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
        });

        services.AddScoped<OrderContextSeed>();
        
        return services;
    }
}