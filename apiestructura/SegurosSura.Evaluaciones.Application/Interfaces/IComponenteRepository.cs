using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IComponenteRepository
{
    Task<IEnumerable<Componente>> GetAllAsync();
    Task<Componente?> GetByIdAsync(Guid id);
    Task<Componente?> GetByNameAsync(string nombre);
    Task<Componente> CreateAsync(Componente componente);
    Task<Componente> UpdateAsync(Componente componente);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string nombre);
}
