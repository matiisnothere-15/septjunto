using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Infrastructure.Repositories
{
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly EvaluacionesDbContext _context;

        public ProyectoRepository(EvaluacionesDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------------------------
        // Obtiene todos los proyectos (solo lectura, sin tracking)
        // ----------------------------------------------------------------------
        public async Task<IEnumerable<Proyecto>> GetAllAsync()
        {
            return await _context.Proyectos
                .AsNoTracking()
                .ToListAsync();
        }

        // ----------------------------------------------------------------------
        // Busca un proyecto por su ID
        // ----------------------------------------------------------------------
        public async Task<Proyecto?> GetByIdAsync(Guid id)
        {
            return await _context.Proyectos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ----------------------------------------------------------------------
        // NUEVO: Busca un proyecto por su nombre (ignora mayúsculas/minúsculas)
        // ----------------------------------------------------------------------
        public async Task<Proyecto?> GetByNameAsync(string nombre)
        {
            return await _context.Proyectos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == nombre.ToLower());
        }

        // ----------------------------------------------------------------------
        // Agrega un nuevo proyecto
        // ----------------------------------------------------------------------
        public async Task<Proyecto> AddAsync(Proyecto proyecto)
        {
            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync();
            return proyecto;
        }

        // ----------------------------------------------------------------------
        // Actualiza un proyecto existente
        // ----------------------------------------------------------------------
        public async Task UpdateAsync(Proyecto proyecto)
        {
            _context.Entry(proyecto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------------------------------------
        // Elimina un proyecto por su ID
        // ----------------------------------------------------------------------
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
}
