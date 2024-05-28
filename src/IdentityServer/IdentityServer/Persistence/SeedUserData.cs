using System.Security.Claims;
using IdentityModel;
using IdentityServer.Infrastructure.Common;
using IdentityServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence;

public static class SeedUserData
{
    public static void EnsureSeedData(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<IdentityContext>(options => { options.UseSqlServer(connectionString); });
        services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                CreateUser(scope, "Alice", "Smith", "1 London Road", Guid.NewGuid().ToString(), "alice123", "admin",
                    "alicesmith@example.com");
            }
        }
    }

    private static void CreateUser(IServiceScope scope, string firstName, string lastName, string address, string id,
        string password, string role, string email)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = userManager.FindByNameAsync(email).Result;
        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Id = id,
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };
            
            var createUserResult = userManager.CreateAsync(user, password).GetAwaiter().GetResult();
            CheckResult(createUserResult);
            
            var addToRoleResult = userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
            CheckResult(addToRoleResult);
            
            var addClaimsResult = userManager.AddClaimsAsync(user, new Claim[]
            {
                new(SystemConstants.Claims.UserName, user.UserName),
                new(SystemConstants.Claims.FirstName, firstName),
                new(SystemConstants.Claims.LastName, lastName),
                new(SystemConstants.Claims.Roles, role),
                new(JwtClaimTypes.Address, address),
                new(JwtClaimTypes.Email, email),
                new(ClaimTypes.NameIdentifier, user.Id)
            }).GetAwaiter().GetResult();
            CheckResult(addClaimsResult);
        }
    }

    private static void CheckResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }
}