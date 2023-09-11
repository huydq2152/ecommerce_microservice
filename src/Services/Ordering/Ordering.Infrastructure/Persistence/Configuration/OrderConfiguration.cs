using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;
using Shared.Enums;

namespace Ordering.Infrastructure.Persistence.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.Status).HasDefaultValue(EOrderStatus.New).IsRequired();
    }
}