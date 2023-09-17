using Microsoft.EntityFrameworkCore;

namespace Product.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var service = scope.ServiceProvider;
            var logger = service.GetRequiredService<ILogger<TContext>>();
            var context = service.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrate mysql database");
                ExecuteMigrations(context);
                logger.LogInformation("Migrated mysql database");
                InvokeSeeder(seeder, context, service);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred when migrating mysql database");
            }
        }

        return host;
    }

    private static void ExecuteMigrations<TContext>(TContext context) where TContext : DbContext
    {
        context.Database.Migrate();
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider serviceProvider) where TContext: DbContext
    {
        seeder(context, serviceProvider);
    }
}