using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System.Threading; // <-- Añadido

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class ComponenteRepository : IComponenteRepository
{
    private readonly EvaluacionesDbContext _context;

    public ComponenteRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Componente>> GetAllAsync(CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Componentes
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken); // <-- Actualizado
    }

    public async Task<Componente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Componentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<Componente?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Componentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower(), cancellationToken); // <-- Actualizado
    }

    public async Task<Componente> CreateAsync(Componente componente, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.Componentes.Add(componente);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return componente;
    }

    public async Task<Componente> UpdateAsync(Componente componente, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        _context.Componentes.Update(componente);
        await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        return componente;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        var componente = await _context.Componentes.FindAsync(new object[] { id }, cancellationToken); // <-- Actualizado
        if (componente != null)
        {
            _context.Componentes.Remove(componente); // Hard delete
            await _context.SaveChangesAsync(cancellationToken); // <-- Actualizado
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Componentes.AsNoTracking().AnyAsync(c => c.Id == id, cancellationToken); // <-- Actualizado
    }

    public async Task<bool> ExistsByNameAsync(string nombre, CancellationToken cancellationToken = default) // <-- Actualizado
    {
        return await _context.Componentes.AsNoTracking().AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower(), cancellationToken); // <-- Actualizado
    }
}