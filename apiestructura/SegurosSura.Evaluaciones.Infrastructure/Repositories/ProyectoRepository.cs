using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories;

public class ProyectoRepository : IProyectoRepository
{
    private readonly EvaluacionesDbContext _context;

    public ProyectoRepository(EvaluacionesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Proyecto>> GetAllAsync()
    {
        return await _context.Proyectos.AsNoTracking().ToListAsync();
    }

    public async Task<Proyecto?> GetByIdAsync(Guid id)
    {
        return await _context.Proyectos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Proyecto> AddAsync(Proyecto proyecto)
    {
        _context.Proyectos.Add(proyecto);
        await _context.SaveChangesAsync();
        return proyecto;
    }

    public async Task UpdateAsync(Proyecto proyecto)
    {
        _context.Entry(proyecto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var proyecto = await _context.Proyectos.FindAsync(id);
        if (proyecto != null)
        {
            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();
        }
    }
}