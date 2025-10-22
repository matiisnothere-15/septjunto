using SegurosSura.Evaluaciones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading; // <-- 1. Añade este using
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Application.Interfaces
{
    public interface IProyectoRepository
    {
        // 2. Añade CancellationToken a todas las firmas de métodos
        Task<IEnumerable<Proyecto>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task<Proyecto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        
        // 3. Asegúrate de que este método (el que soluciona el error 500) esté aquí
        Task<Proyecto?> GetByNameAsync(string nombre, CancellationToken cancellationToken = default);
        
        Task<Proyecto> AddAsync(Proyecto proyecto, CancellationToken cancellationToken = default);
        
        Task UpdateAsync(Proyecto proyecto, CancellationToken cancellationToken = default);
        
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}