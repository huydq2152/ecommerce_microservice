using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence;

public static class IdentitySeed
{
    public static async Task<IHost> MigrateDatabaseAsync(this IHost host, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        using var scope = host.Services.CreateScope();

        await using var configurationDbContext = scope.ServiceProvider
            .GetRequiredService<ConfigurationDbContext>();
        configurationDbContext.Database.SetConnectionString(connectionString);
        await configurationDbContext.Database.MigrateAsync();

        await using var persistedGrantDbContext = scope.ServiceProvider
            .GetRequiredService<PersistedGrantDbContext>();
        persistedGrantDbContext.Database.SetConnectionString(connectionString);
        await persistedGrantDbContext.Database.MigrateAsync();

        await using var identityContext = scope.ServiceProvider
           .GetRequiredService<IdentityContext>();
        identityContext.Database.SetConnectionString(connectionString);
        await identityContext.Database.MigrateAsync();
        try
        {
            if (!configurationDbContext.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    configurationDbContext.Clients.Add(client.ToEntity());
                }
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    configurationDbContext.IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!configurationDbContext.ApiScopes.Any())
            {
                foreach (var apiScope in Config.ApiScopes)
                {
                    configurationDbContext.ApiScopes.Add(apiScope.ToEntity());
                }
            }

            if (!configurationDbContext.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    configurationDbContext.ApiResources.Add(resource.ToEntity());
                }
            }

            await configurationDbContext.SaveChangesAsync();
            await identityContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return host;
    }
}