using IdentityServer4.AccessTokenValidation;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;

namespace Infrastructure.Identity;

public static class ConfigureAuthAuthorHandler
{
    public static void ConfigureAuthenticationHandler(this IServiceCollection services)
    {
        var configuration = services.GetOption<ApiConfiguration>("ApiConfiguration");
        if (configuration == null || string.IsNullOrEmpty(configuration.IssuerUri) ||
            string.IsNullOrEmpty(configuration.ApiName))
        {
            throw new Exception("Api configuration is not configured");
        }

        var issuerUri = configuration.IssuerUri;
        var apiName = configuration.ApiName;

        services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = issuerUri;
                options.ApiName = apiName;
                options.RequireHttpsMetadata = false;
                options.SupportedTokens = SupportedTokens.Both;
            });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(IdentityServerAuthenticationDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });
    }
}