using System.Reflection;
using Contracts.Domains.Interface;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext: DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options): base(options)
    {
        
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var modified = ChangeTracker.Entries().Where(o =>
            o.State == EntityState.Added || o.State == EntityState.Modified || o.State == EntityState.Deleted);
        foreach (var item in modified)
        {
            switch (item.State)
            {
                case EntityState.Added:
                    if (item.Entity is IDateTracking addedEntity)
                    {
                        addedEntity.CreatedDate = DateTimeOffset.UtcNow;
                        item.State = EntityState.Added;
                    }
                    break;
                case EntityState.Modified:
                    Entry(item.Entity).Property("Id").IsModified = false; // id can't modified when ust SaveChange()
                    if (item.Entity is IDateTracking modifiedEntity)
                    {
                        modifiedEntity.LastModifiedDate = DateTimeOffset.UtcNow;
                        item.State = EntityState.Modified;
                    }
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}