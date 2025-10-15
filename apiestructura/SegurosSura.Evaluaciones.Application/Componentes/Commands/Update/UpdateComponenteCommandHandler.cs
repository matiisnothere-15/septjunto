using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Update;

public class UpdateComponenteCommandHandler : IRequestHandler<UpdateComponenteCommand>
{
    private readonly IComponenteRepository _componenteRepository;
    private readonly IProyectoRepository _proyectoRepository;

    public UpdateComponenteCommandHandler(IComponenteRepository componenteRepository, IProyectoRepository proyectoRepository)
    {
        _componenteRepository = componenteRepository;
        _proyectoRepository = proyectoRepository;
    }

    public async Task Handle(UpdateComponenteCommand request, CancellationToken cancellationToken)
    {
        var componente = await _componenteRepository.GetByIdAsync(request.Id);
        if (componente == null)
        {
            throw new EntityNotFoundException("Componente", request.Id);
        }

        // Verificar si ya existe otro componente con el mismo nombre
        var existingComponente = await _componenteRepository.GetByNameAsync(request.Nombre);
        if (existingComponente != null && existingComponente.Id != request.Id)
        {
            throw new EntityAlreadyExistsException("Componente", "nombre", request.Nombre);
        }

        componente.Nombre = request.Nombre;
        componente.Descripcion = request.Descripcion;

        if (request.ProyectoId.HasValue)
        {
            var proyecto = await _proyectoRepository.GetByIdAsync(request.ProyectoId.Value);
            if (proyecto == null)
            {
                throw new EntityNotFoundException("Proyecto", request.ProyectoId.Value);
            }
            componente.ProyectoId = request.ProyectoId.Value;
        }

        await _componenteRepository.UpdateAsync(componente);
    }
}
