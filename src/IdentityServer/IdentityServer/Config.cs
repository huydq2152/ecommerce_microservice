using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new()
            {
                Name = "roles",
                DisplayName = "User role(s)",
                UserClaims = new List<string> { "roles" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("microservices_api.read", "Microservices API Read Scope"),
            new("microservices_api.write", "Microservices API Write Scope"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("microservices_api", "Microservices API")
            {
                Scopes = new List<string> { "microservices_api.read", "microservices_api.write" },
                UserClaims = new List<string> { "roles" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientName = "Microservices Swagger Client",
                ClientId = "microservices_swagger",

                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,

                RedirectUris = new List<string>()
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                },
                AllowedCorsOrigins = new List<string>()
                {
                    "http://localhost:5001",
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "microservices_api.read",
                    "microservices_api.write",
                    "microservices_api",
                    "roles"
                }
            },
            new()
            {
                ClientName = "Microservices Postman Client",
                ClientId = "microservices_postman",
                Enabled = true,
                ClientUri = null,
                RequireClientSecret = true,
                ClientSecrets = new[]
                {
                    new Secret("SuperStrongSecret".Sha512())
                },
                AllowedGrantTypes = new[]
                {
                    GrantType.ClientCredentials,
                    GrantType.ResourceOwnerPassword
                },
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    "https://www.getpostman.com/oauth2/callback"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "microservices_api.read",
                    "microservices_api.write",
                    "microservices_api",
                }
            },
        };
}