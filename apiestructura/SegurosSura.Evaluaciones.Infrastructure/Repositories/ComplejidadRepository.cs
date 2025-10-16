using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class ComplejidadRepository : IComplejidadRepository
{
    private readonly EvaluacionesDbContext _context;

    public ComplejidadRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Complejidad>> GetAllAsync()
    {
        return await _context.Complejidades
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Orden)
            .ToListAsync();
    }

    public async Task<Complejidad?> GetByIdAsync(Guid id)
    {
        return await _context.Complejidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Complejidad?> GetByNameAsync(string nombre)
    {
        return await _context.Complejidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
    }

    public async Task<Complejidad> CreateAsync(Complejidad complejidad)
    {
        _context.Complejidades.Add(complejidad);
        await _context.SaveChangesAsync();
        return complejidad;
    }

    public async Task<Complejidad> UpdateAsync(Complejidad complejidad)
    {
        _context.Complejidades.Update(complejidad);
        await _context.SaveChangesAsync();
        return complejidad;
    }

    public async Task DeleteAsync(Guid id)
    {
        var complejidad = await _context.Complejidades.FindAsync(id);
        if (complejidad != null)
        {
            complejidad.Activo = false; // Soft delete
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Complejidades.AsNoTracking().AnyAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string nombre)
    {
        return await _context.Complejidades.AsNoTracking().AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
    }

    public async Task<bool> ExistsByOrderAsync(int orden)
    {
        return await _context.Complejidades.AsNoTracking().AnyAsync(c => c.Orden == orden);
    }
}
