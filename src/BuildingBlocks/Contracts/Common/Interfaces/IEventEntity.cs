using Contracts.Common.Events;
using Contracts.Domains.Interface;

namespace Contracts.Common.Interfaces;

public interface IEventEntity
{
    void AddDomainEvents(BaseEvent domainEvent);
    void RemoveDomainEvents(BaseEvent domainEvent);
    void ClearDomainEvents();

    IReadOnlyCollection<BaseEvent> DomainEvents();
}

public interface IEventEntity<T> : IEntityBase<T>, IEventEntity
{
}