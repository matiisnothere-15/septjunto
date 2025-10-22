using SegurosSura.Evaluaciones.Domain.Entities;
using System.Threading; // <-- Añadido
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IComplejidadRepository
{
    Task<IEnumerable<Complejidad>> GetAllAsync(CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Complejidad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Complejidad?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Complejidad> CreateAsync(Complejidad complejidad, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Complejidad> UpdateAsync(Complejidad complejidad, CancellationToken cancellationToken = default); // <-- Actualizado
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<bool> ExistsByNameAsync(string nombre, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<bool> ExistsByOrderAsync(int orden, CancellationToken cancellationToken = default); // <-- Actualizado
}