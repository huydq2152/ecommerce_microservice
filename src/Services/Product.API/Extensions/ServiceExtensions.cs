using System.Text;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        if (databaseSettings == null) throw new ArgumentNullException("database settings is not configured");
        services.AddSingleton(databaseSettings);
        
        var apiConfiguration = configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
        if (apiConfiguration == null) throw new ArgumentNullException("api configuration is not configured");
        services.AddSingleton(apiConfiguration);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigurationSettings(configuration);

        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();

        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        // services.AddJwtAuthentication();
        services.ConfigureAuthenticationHandler();
        services.ConfigureAuthorization();

        services.ConfigureHealthChecks();

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
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
            throw new ArgumentNullException("Connection string is not configured.");

        var builder =
            new MySqlConnectionStringBuilder(databaseSettings
                .ConnectionString); //build standard connection string of mysql

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

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var databaseSettings = services.GetOption<DatabaseSettings>(nameof(DatabaseSettings));
        services.AddHealthChecks()
            .AddMySql(databaseSettings.ConnectionString, name: "ProductDb-mysql-check",
                failureStatus: HealthStatus.Degraded);
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        var configuration = services.GetOption<ApiConfiguration>("ApiConfiguration");
        if (configuration == null || string.IsNullOrEmpty(configuration.IssuerUri) ||
            string.IsNullOrEmpty(configuration.ApiName)) throw new Exception("ApiConfiguration is not configured!");

        services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Product API V1",
                        Version = configuration.ApiVersion,
                    });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{configuration.IdentityServerBaseUrl}/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "microservices_api.read", "Microservices API Read Scope" },
                                { "microservices_api.write", "Microservices API Write Scope" }
                            }
                        }
                    }
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Name = JwtBearerDefaults.AuthenticationScheme
                        },
                        new List<string>
                        {
                            "microservices_api.read", 
                            "microservices_api.write"
                        }
                    }
                });
            });
        
    }
}