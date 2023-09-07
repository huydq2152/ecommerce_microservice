using Contracts.Domains.Interface;

namespace Contracts.Domains;

public abstract class EntityBase<T> : IEntityBase<T>
{
    public T Id { get; set; }
}