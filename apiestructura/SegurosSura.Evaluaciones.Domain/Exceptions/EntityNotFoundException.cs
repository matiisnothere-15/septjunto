using System;

namespace SegurosSura.Evaluaciones.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, Guid id)
            : base($"{entityName} con ID {id} no fue encontrado.")
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string entityName, string fieldName, string fieldValue)
            : base($"{entityName} con {fieldName} = '{fieldValue}' no fue encontrado.")
        {
        }
    }
}
