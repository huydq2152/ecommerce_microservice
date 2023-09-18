namespace Contracts.Domains.Interface;

public interface IEntityBase<T>
{
    T Id { get; set; }
}