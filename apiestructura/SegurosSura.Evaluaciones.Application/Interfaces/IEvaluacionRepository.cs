using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IEvaluacionRepository
{
    Task<IEnumerable<Evaluacion>> GetAllAsync();
    Task<Evaluacion?> GetByIdAsync(Guid id);
    Task<Evaluacion> CreateAsync(Evaluacion evaluacion);
    Task<Evaluacion> UpdateAsync(Evaluacion evaluacion);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
