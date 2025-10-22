using SegurosSura.Evaluaciones.Domain.Entities;
using System.Threading; // <-- 1. Añade este using
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IRelacionComponenteComplejidadRepository
{
    // 2. Añade CancellationToken a todas las firmas de métodos
    Task<IEnumerable<RelacionComponenteComplejidad>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<RelacionComponenteComplejidad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<RelacionComponenteComplejidad?> GetByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RelacionComponenteComplejidad>> GetByComponenteAsync(Guid componenteId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<RelacionComponenteComplejidad>> GetByComplejidadAsync(Guid complejidadId, CancellationToken cancellationToken = default);
    
    Task<RelacionComponenteComplejidad> CreateAsync(RelacionComponenteComplejidad relacion, CancellationToken cancellationToken = default);
    
    Task<RelacionComponenteComplejidad> UpdateAsync(RelacionComponenteComplejidad relacion, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId, CancellationToken cancellationToken = default);
}