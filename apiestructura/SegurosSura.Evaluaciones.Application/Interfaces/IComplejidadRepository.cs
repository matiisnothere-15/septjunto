using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IComplejidadRepository
{
    Task<IEnumerable<Complejidad>> GetAllAsync();
    Task<Complejidad?> GetByIdAsync(Guid id);
    Task<Complejidad?> GetByNameAsync(string nombre);
    Task<Complejidad> CreateAsync(Complejidad complejidad);
    Task<Complejidad> UpdateAsync(Complejidad complejidad);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string nombre);
    Task<bool> ExistsByOrderAsync(int orden);
}
