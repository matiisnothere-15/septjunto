using SegurosSura.Evaluaciones.Domain.Entities;
using System.Threading; // <-- Añadido
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IEvaluacionRepository
{
    Task<IEnumerable<Evaluacion>> GetAllAsync(CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Evaluacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Evaluacion> CreateAsync(Evaluacion evaluacion, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Evaluacion> UpdateAsync(Evaluacion evaluacion, CancellationToken cancellationToken = default); // <-- Actualizado
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
}