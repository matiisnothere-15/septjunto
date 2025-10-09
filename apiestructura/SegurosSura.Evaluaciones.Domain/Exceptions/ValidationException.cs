namespace SegurosSura.Evaluaciones.Domain.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
    
    public ValidationException(string field, string message) 
        : base($"Error de validación en {field}: {message}")
    {
    }
}
