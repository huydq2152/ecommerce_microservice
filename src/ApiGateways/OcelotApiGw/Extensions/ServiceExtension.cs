using System.Text;
using Contracts.Identity;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Shared.Configuration;

namespace OcelotApiGw.Extensions;

public static class ServiceExtension
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        if (jwtSettings == null) throw new ArgumentNullException("JWT settings is not configured");
        services.AddSingleton(jwtSettings);
        return services;
    }

    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration);
        services.AddTransient<ITokenService, TokenService>();
        services.AddJwtAuthentication();
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

    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowOrigins"];
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
    
    
}