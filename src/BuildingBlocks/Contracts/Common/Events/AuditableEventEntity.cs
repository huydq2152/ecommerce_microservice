using Contracts.Domains.Interface;

namespace Contracts.Common.Events;

public class AuditableEventEntity<T>: EventEntity<T>, IAuditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}