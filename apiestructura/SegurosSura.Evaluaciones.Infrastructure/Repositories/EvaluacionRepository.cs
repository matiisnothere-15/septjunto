using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System.Threading; // <-- Añadido

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class EvaluacionRepository : IEvaluacionRepository
{
    private readonly EvaluacionesDbContext _context;

    public EvaluacionRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Evaluacion>> GetAllAsync(CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Evaluaciones
            .AsNoTracking()
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Componente)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Complejidad)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync(cancellationToken); // <-- Actualizado
    }

    public async Task<Evaluacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Evaluaciones
            .AsNoTracking()
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Componente)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Complejidad)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<Evaluacion> CreateAsync(Evaluacion evaluacion, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.Evaluaciones.Add(evaluacion);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return evaluacion;
    }

    public async Task<Evaluacion> UpdateAsync(Evaluacion evaluacion, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.Evaluaciones.Update(evaluacion);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return evaluacion;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        var evaluacion = await _context.Evaluaciones.FindAsync(new object[] { id }, cancellationToken); // <-- Actualizado
        if (evaluacion != null)
        {
            _context.Evaluaciones.Remove(evaluacion);
            await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Evaluaciones.AsNoTracking().AnyAsync(e => e.Id == id, cancellationToken); // <-- Actualizado
    }
}