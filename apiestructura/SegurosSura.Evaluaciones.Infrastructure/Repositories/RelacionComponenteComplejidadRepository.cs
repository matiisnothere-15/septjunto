using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class RelacionComponenteComplejidadRepository : IRelacionComponenteComplejidadRepository
{
    private readonly EvaluacionesDbContext _context;

    public RelacionComponenteComplejidadRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> GetAllAsync()
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .ToListAsync();
    }

    public async Task<RelacionComponenteComplejidad?> GetByIdAsync(Guid id)
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<RelacionComponenteComplejidad?> GetByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId)
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .FirstOrDefaultAsync(r => r.ComponenteId == componenteId && r.ComplejidadId == complejidadId);
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> GetByComponenteAsync(Guid componenteId)
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .Where(r => r.ComponenteId == componenteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> GetByComplejidadAsync(Guid complejidadId)
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .Where(r => r.ComplejidadId == complejidadId)
            .ToListAsync();
    }

    public async Task<RelacionComponenteComplejidad> CreateAsync(RelacionComponenteComplejidad relacion)
    {
        _context.RelacionesComponenteComplejidad.Add(relacion);
        await _context.SaveChangesAsync();
        return relacion;
    }

    public async Task<RelacionComponenteComplejidad> UpdateAsync(RelacionComponenteComplejidad relacion)
    {
        _context.RelacionesComponenteComplejidad.Update(relacion);
        await _context.SaveChangesAsync();
        return relacion;
    }

    public async Task DeleteAsync(Guid id)
    {
        var relacion = await _context.RelacionesComponenteComplejidad.FindAsync(id);
        if (relacion != null)
        {
            _context.RelacionesComponenteComplejidad.Remove(relacion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.RelacionesComponenteComplejidad.AsNoTracking().AnyAsync(r => r.Id == id);
    }

    public async Task<bool> ExistsByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId)
    {
        return await _context.RelacionesComponenteComplejidad.AsNoTracking().AnyAsync(r => r.ComponenteId == componenteId && r.ComplejidadId == complejidadId);
    }
}
