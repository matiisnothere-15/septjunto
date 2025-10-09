using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Update;

public class UpdateComplejidadCommandHandler : IRequestHandler<UpdateComplejidadCommand>
{
    private readonly IComplejidadRepository _complejidadRepository;

    public UpdateComplejidadCommandHandler(IComplejidadRepository complejidadRepository)
    {
        _complejidadRepository = complejidadRepository;
    }

    public async Task Handle(UpdateComplejidadCommand request, CancellationToken cancellationToken)
    {
        var complejidad = await _complejidadRepository.GetByIdAsync(request.Id);
        if (complejidad == null)
        {
            throw new EntityNotFoundException("Complejidad", request.Id);
        }

        // Verificar si ya existe otra complejidad con el mismo nombre
        var existingComplejidad = await _complejidadRepository.GetByNameAsync(request.Nombre);
        if (existingComplejidad != null && existingComplejidad.Id != request.Id)
        {
            throw new EntityAlreadyExistsException("Complejidad", "nombre", request.Nombre);
        }

        // Verificar si ya existe otra complejidad con el mismo orden
        var existingOrden = await _complejidadRepository.ExistsByOrderAsync(request.Orden);
        if (existingOrden)
        {
            var complejidadConOrden = await _complejidadRepository.GetAllAsync();
            var conflicto = complejidadConOrden.FirstOrDefault(c => c.Orden == request.Orden && c.Id != request.Id);
            if (conflicto != null)
            {
                throw new EntityAlreadyExistsException("Complejidad", "orden", request.Orden.ToString());
            }
        }

        complejidad.Nombre = request.Nombre;
        complejidad.Orden = request.Orden;

        await _complejidadRepository.UpdateAsync(complejidad);
    }
}
