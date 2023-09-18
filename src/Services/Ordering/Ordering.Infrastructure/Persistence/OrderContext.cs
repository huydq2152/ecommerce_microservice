using System.Reflection;
using Contracts.Common.Events;
using Contracts.Common.Interfaces;
using Contracts.Domains.Interface;
using Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Serilog;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public OrderContext(DbContextOptions<OrderContext> options, IMediator mediator, ILogger logger) : base(options)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public DbSet<Order> Orders { get; set; }

    private List<BaseEvent> BaseEvents { get; set; }

    private void SetBaseEventsBeforeSaveChange()
    {
        var domainEntities = ChangeTracker.Entries<IEventEntity>()
            .Select(o => o.Entity)
            .Where(o => o.DomainEvents().Any()).ToList();

        BaseEvents = domainEntities.SelectMany(o => o.DomainEvents()).ToList();
        domainEntities.ForEach(o => o.ClearDomainEvents());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetBaseEventsBeforeSaveChange();
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

        var result = await base.SaveChangesAsync(cancellationToken);
        await _mediator.DispatchDomainEventAsync(BaseEvents, _logger);

        return result;
    }
}