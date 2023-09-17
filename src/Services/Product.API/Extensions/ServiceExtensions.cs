using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        return services;
    }

    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        var builder = new MySqlConnectionStringBuilder(connectionString); //build standard connection string of mysql

        services.AddDbContext<ProductContext>(optionsBuilder => optionsBuilder.UseMySql(builder.ConnectionString,
            serverVersion: ServerVersion.AutoDetect(builder.ConnectionString), contextOptionsBuilder =>
            {
                contextOptionsBuilder.MigrationsAssembly("Product.API");
                contextOptionsBuilder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
            .AddScoped<IProductRepository, ProductRepository>();
    }
}