namespace Ordering.Domain.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entity, object key): base($"Entity \"{entity}\" ({key}) was not found.")
    {
        
    }
}