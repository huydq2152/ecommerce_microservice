using IdentityServer.Infrastructure.Entities;
using IdentityServer.Persistence;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace IdentityServer.Extensions;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection(nameof(SmtpEmailSetting)).Get<SmtpEmailSetting>();
        if (emailSettings == null)
        {
            throw new ArgumentNullException("SmtpEmailSetting is not configured.");
        }

        services.AddSingleton(emailSettings);

        return services;
    }

    public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("IdentitySqlConnection is not configured.");
        }

        services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            // not create signing credential in production
            .AddDeveloperSigningCredential()
            // .AddInMemoryIdentityResources(Config.IdentityResources)
            // .AddInMemoryApiScopes(Config.ApiScopes)
            // .AddInMemoryClients(Config.Clients)
            // .AddInMemoryApiResources(Config.ApiResources)
            // .AddTestUsers(TestUsers.Users)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(
                    connectionString,
                    optionsBuilder => optionsBuilder.MigrationsAssembly("IdentityServer"));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(
                    connectionString,
                    optionsBuilder => optionsBuilder.MigrationsAssembly("IdentityServer"));
            })
            .AddAspNetIdentity<User>()
            .AddProfileService<IdentityProfileService>();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("IdentitySqlConnection is not configured.");
        }

        services
            .AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString))
            .AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddUserStore<UserStore>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.EnableAnnotations();
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Identity Server API",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Identity Service",
                    Email = "quanghuy2152@gmail.com",
                    Url = new Uri("https://huydang.dev")
                },
            });
            
            var identityServerBaseUrl = configuration.GetSection("IdentityServer:BaseUrl").Value;

            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityServerBaseUrl}/connect/authorize"),
                        Scopes = new Dictionary<string, string>()
                        {
                            {"microservices_api.read", "Microservices API Read Scope"},
                            {"microservices_api.write", "Microservices API Write Scope"}
                        }
                    }
                }
            });
            config.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                    {
                        "microservices_api.read",
                        "microservices_api.write"
                    }
                }
            });
        });
    }
}