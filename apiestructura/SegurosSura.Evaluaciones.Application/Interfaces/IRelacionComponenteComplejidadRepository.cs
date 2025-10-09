using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IRelacionComponenteComplejidadRepository
{
    Task<IEnumerable<RelacionComponenteComplejidad>> GetAllAsync();
    Task<RelacionComponenteComplejidad?> GetByIdAsync(Guid id);
    Task<RelacionComponenteComplejidad?> GetByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId);
    Task<IEnumerable<RelacionComponenteComplejidad>> GetByComponenteAsync(Guid componenteId);
    Task<IEnumerable<RelacionComponenteComplejidad>> GetByComplejidadAsync(Guid complejidadId);
    Task<RelacionComponenteComplejidad> CreateAsync(RelacionComponenteComplejidad relacion);
    Task<RelacionComponenteComplejidad> UpdateAsync(RelacionComponenteComplejidad relacion);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId);
}
