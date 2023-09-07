using Contracts.Domains.Interface;

namespace Contracts.Domains;

public abstract class EntityAuditBase<T> : EntityBase<T>, IAuditable
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? LastModifiedDate { get; set; }
}