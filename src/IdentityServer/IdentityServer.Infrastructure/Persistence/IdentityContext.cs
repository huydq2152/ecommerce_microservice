using IdentityServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence;

public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyIdentityConfiguration();
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
    }
}