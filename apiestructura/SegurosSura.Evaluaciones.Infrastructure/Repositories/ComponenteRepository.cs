using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class ComponenteRepository : IComponenteRepository
{
    private readonly EvaluacionesDbContext _context;

    public ComponenteRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Componente>> GetAllAsync()
    {
        return await _context.Componentes
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Componente?> GetByIdAsync(Guid id)
    {
        return await _context.Componentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Componente?> GetByNameAsync(string nombre)
    {
        return await _context.Componentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
    }

    public async Task<Componente> CreateAsync(Componente componente)
    {
        _context.Componentes.Add(componente);
        await _context.SaveChangesAsync();
        return componente;
    }

    public async Task<Componente> UpdateAsync(Componente componente)
    {
        _context.Componentes.Update(componente);
        await _context.SaveChangesAsync();
        return componente;
    }

    public async Task DeleteAsync(Guid id)
    {
        var componente = await _context.Componentes.FindAsync(id);
        if (componente != null)
        {
            componente.Activo = false; // Soft delete
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Componentes.AsNoTracking().AnyAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string nombre)
    {
        return await _context.Componentes.AsNoTracking().AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
    }
}
