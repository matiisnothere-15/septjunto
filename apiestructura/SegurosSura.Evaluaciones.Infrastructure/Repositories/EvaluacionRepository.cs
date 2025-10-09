using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class EvaluacionRepository : IEvaluacionRepository
{
    private readonly EvaluacionesDbContext _context;

    public EvaluacionRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Evaluacion>> GetAllAsync()
    {
        return await _context.Evaluaciones
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Componente)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Complejidad)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<Evaluacion?> GetByIdAsync(Guid id)
    {
        return await _context.Evaluaciones
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Componente)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Complejidad)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Evaluacion> CreateAsync(Evaluacion evaluacion)
    {
        _context.Evaluaciones.Add(evaluacion);
        await _context.SaveChangesAsync();
        return evaluacion;
    }

    public async Task<Evaluacion> UpdateAsync(Evaluacion evaluacion)
    {
        _context.Evaluaciones.Update(evaluacion);
        await _context.SaveChangesAsync();
        return evaluacion;
    }

    public async Task DeleteAsync(Guid id)
    {
        var evaluacion = await _context.Evaluaciones.FindAsync(id);
        if (evaluacion != null)
        {
            _context.Evaluaciones.Remove(evaluacion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Evaluaciones.AnyAsync(e => e.Id == id);
    }
}
