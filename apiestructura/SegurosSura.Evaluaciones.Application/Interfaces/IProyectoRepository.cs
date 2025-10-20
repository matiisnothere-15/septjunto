using SegurosSura.Evaluaciones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Application.Interfaces;

public interface IProyectoRepository
{
    Task<IEnumerable<Proyecto>> GetAllAsync();
    Task<Proyecto?> GetByIdAsync(Guid id);
    Task<Proyecto> AddAsync(Proyecto proyecto);
    Task UpdateAsync(Proyecto proyecto);
    Task DeleteAsync(Guid id);
    Task<Proyecto?> GetByNameAsync(string nombre);
}