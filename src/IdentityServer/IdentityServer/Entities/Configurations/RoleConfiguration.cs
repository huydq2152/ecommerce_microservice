using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Entities.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole()
        {
            Name = "Admin",
            NormalizedName = "ADMIN",
            Id = Guid.NewGuid().ToString(),
        }, new IdentityRole()
        {
            Name = "Customer",
            NormalizedName = "CUSTOMER",
            Id = Guid.NewGuid().ToString(),
        });
    }
}