using IdentityServer.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Infrastructure.Entities.Configurations;

public class PermissionConfiguration: IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", SystemConstants.IdentitySchema)
            .HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.HasIndex(o => new { o.RoleId, o.Function, o.Command })
            .IsUnique();
    }
}