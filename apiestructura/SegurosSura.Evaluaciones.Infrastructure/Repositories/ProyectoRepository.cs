using Microsoft.EntityFrameworkCore;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading; // Necesario para CancellationToken
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
        // - Añadido CancellationToken
        // ----------------------------------------------------------------------
        public async Task<IEnumerable<Proyecto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Proyectos
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // ----------------------------------------------------------------------
        // Busca un proyecto por su ID
        // - Añadido CancellationToken
        // ----------------------------------------------------------------------
        public async Task<Proyecto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Proyectos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        // ----------------------------------------------------------------------
        // MEJORADO: Busca un proyecto por su nombre
        // - Añadido CancellationToken
        // - Cambiada la consulta para ser más eficiente y correcta.
        // ----------------------------------------------------------------------
        public async Task<Proyecto?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return null;
            }

            // 1. Normalizamos el nombre (quitamos espacios)
            var nombreNormalizado = nombre.Trim();

            return await _context.Proyectos
                .AsNoTracking()
                // 2. Usamos Equals con StringComparison para forzar la comparación
                //    sin importar mayúsculas/minúsculas.
                //    Esto se traduce a UPPER(p.Nombre) == UPPER(@nombreNormalizado)
                //    en SQL, lo cual es mucho más eficiente y puede usar índices
                //    que tu versión anterior con ToLower() y Trim() en la columna.
                .FirstOrDefaultAsync(p => p.Nombre.Equals(nombreNormalizado, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
        }

        // ----------------------------------------------------------------------
        // Agrega un nuevo proyecto
        // - Añadido CancellationToken
        // - Añadido chequeo de nulidad.
        // ----------------------------------------------------------------------
        public async Task<Proyecto> AddAsync(Proyecto proyecto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(proyecto);

            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync(cancellationToken);
            return proyecto;
        }

        // ----------------------------------------------------------------------
        // MEJORADO: Actualiza un proyecto existente
        // - Añadido CancellationToken
        // - Cambiado a un método más seguro que evita problemas de tracking.
        // ----------------------------------------------------------------------
        public async Task UpdateAsync(Proyecto proyecto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(proyecto);

            // Esta es una forma más segura de actualizar.
            // Attach primero y luego marca el estado como modificado.
            // _context.Proyectos.Update(proyecto); también es una alternativa válida.
            _context.Proyectos.Attach(proyecto);
            _context.Entry(proyecto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Manejar el caso donde el registro fue modificado o borrado 
                // por otro usuario mientras intentábamos guardar.
                Console.WriteLine($"Error de concurrencia actualizando proyecto: {ex.Message}");
                // Podrías querer lanzar una excepción personalizada aquí
                throw; 
            }
        }

        // ----------------------------------------------------------------------
        // Elimina un proyecto por su ID
        // - Añadido CancellationToken
        // ----------------------------------------------------------------------
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var proyecto = await _context.Proyectos.FindAsync(new object[] { id }, cancellationToken);
            if (proyecto != null)
            {
                _context.Proyectos.Remove(proyecto);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}