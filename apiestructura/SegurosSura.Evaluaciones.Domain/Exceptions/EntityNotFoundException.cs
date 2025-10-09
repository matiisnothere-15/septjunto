namespace SegurosSura.Evaluaciones.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, Guid id) 
        : base($"{entityName} con ID {id} no fue encontrado.")
    {
    }
    
    public EntityNotFoundException(string message) : base(message)
    {
    }
}
