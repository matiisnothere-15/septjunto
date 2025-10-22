using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System.Threading; // <-- Añadido

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class ComplejidadRepository : IComplejidadRepository
{
    private readonly EvaluacionesDbContext _context;

    public ComplejidadRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Complejidad>> GetAllAsync(CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Complejidades
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Orden)
            .ToListAsync(cancellationToken); // <-- Actualizado
    }

    public async Task<Complejidad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Complejidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<Complejidad?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Complejidades
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower(), cancellationToken); // <-- Actualizado
    }

    public async Task<Complejidad> CreateAsync(Complejidad complejidad, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.Complejidades.Add(complejidad);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return complejidad;
    }

    public async Task<Complejidad> UpdateAsync(Complejidad complejidad, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.Complejidades.Update(complejidad);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return complejidad;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        var complejidad = await _context.Complejidades.FindAsync(new object[] { id }, cancellationToken); // <-- Actualizado
        if (complejidad != null)
        {
            complejidad.Activo = false; // Soft delete
            await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Complejidades.AsNoTracking().AnyAsync(c => c.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<bool> ExistsByNameAsync(string nombre, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Complejidades.AsNoTracking().AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower(), cancellationToken); // <-- Actualizado
    }

    public async Task<bool> ExistsByOrderAsync(int orden, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Complejidades.AsNoTracking().AnyAsync(c => c.Orden == orden, cancellationToken); // <-- Actualizado
    }
}