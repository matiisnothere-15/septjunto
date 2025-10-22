using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System.Threading; // <-- Añadido

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class RelacionComponenteComplejidadRepository : IRelacionComponenteComplejidadRepository
{
    private readonly EvaluacionesDbContext _context;

    public RelacionComponenteComplejidadRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> GetAllAsync(CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .ToListAsync(cancellationToken); // <-- Actualizado
    }

    public async Task<RelacionComponenteComplejidad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<RelacionComponenteComplejidad?> GetByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .FirstOrDefaultAsync(r => r.ComponenteId == componenteId && r.ComplejidadId == complejidadId, cancellationToken); // <-- Actualizado
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> GetByComponenteAsync(Guid componenteId, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .Where(r => r.ComponenteId == componenteId)
            .ToListAsync(cancellationToken); // <-- Actualizado
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> GetByComplejidadAsync(Guid complejidadId, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad
            .AsNoTracking()
            .Include(r => r.Componente)
            .Include(r => r.Complejidad)
            .Where(r => r.ComplejidadId == complejidadId)
            .ToListAsync(cancellationToken); // <-- Actualizado
    }

    public async Task<RelacionComponenteComplejidad> CreateAsync(RelacionComponenteComplejidad relacion, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.RelacionesComponenteComplejidad.Add(relacion);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return relacion;
    }

    public async Task<RelacionComponenteComplejidad> UpdateAsync(RelacionComponenteComplejidad relacion, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.RelacionesComponenteComplejidad.Update(relacion);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return relacion;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        var relacion = await _context.RelacionesComponenteComplejidad.FindAsync(new object[] { id }, cancellationToken); // <-- Actualizado
        if (relacion != null)
        {
            _context.RelacionesComponenteComplejidad.Remove(relacion);
            await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad.AsNoTracking().AnyAsync(r => r.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<bool> ExistsByComponenteAndComplejidadAsync(Guid componenteId, Guid complejidadId, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.RelacionesComponenteComplejidad.AsNoTracking().AnyAsync(r => r.ComponenteId == componenteId && r.ComplejidadId == complejidadId, cancellationToken); // <-- Actualizado
    }
}