namespace SegurosSura.Evaluaciones.Domain.Exceptions;

public class EntityAlreadyExistsException : Exception
{
    public EntityAlreadyExistsException(string entityName, string field, string value) 
        : base($"{entityName} con {field} '{value}' ya existe.")
    {
    }
    
    public EntityAlreadyExistsException(string message) : base(message)
    {
    }
}
