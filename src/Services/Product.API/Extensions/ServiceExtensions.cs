using System.Text;
using Contracts.Common.Interfaces;
using Contracts.Identity;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configuration;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        if (jwtSettings == null) throw new ArgumentNullException("JWT settings is not configured");
        services.AddSingleton(jwtSettings);
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigurationSettings(configuration);

        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        services.AddJwtAuthentication();

        return services;
    }

    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var settings = services.GetOption<JwtSettings>(nameof(JwtSettings));
        if (settings == null || string.IsNullOrEmpty(settings.Key))
            throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = false
        };
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = tokenValidationParameters;
        });

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