using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Events;

public class EventEntity<T> : EntityBase<T>, IEventEntity<T>
{
    private readonly List<BaseEvent> _domainEvents = new List<BaseEvent>();

    public void AddDomainEvents(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvents(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public IReadOnlyCollection<BaseEvent> DomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }
}