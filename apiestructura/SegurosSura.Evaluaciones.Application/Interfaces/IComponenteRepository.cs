using SegurosSura.Evaluaciones.Domain.Entities;
using System.Threading; // <-- Añadido
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IComponenteRepository
{
    Task<IEnumerable<Componente>> GetAllAsync(CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Componente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Componente?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Componente> CreateAsync(Componente componente, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<Componente> UpdateAsync(Componente componente, CancellationToken cancellationToken = default); // <-- Actualizado
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default); // <-- Actualizado
    Task<bool> ExistsByNameAsync(string nombre, CancellationToken cancellationToken = default); // <-- Actualizado
}