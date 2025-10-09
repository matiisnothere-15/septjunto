using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Create;

public class CreateRelacionCommandHandler : IRequestHandler<CreateRelacionCommand, Guid>
{
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;
    private readonly IComponenteRepository _componenteRepository;
    private readonly IComplejidadRepository _complejidadRepository;

    public CreateRelacionCommandHandler(
        IRelacionComponenteComplejidadRepository relacionRepository,
        IComponenteRepository componenteRepository,
        IComplejidadRepository complejidadRepository)
    {
        _relacionRepository = relacionRepository;
        _componenteRepository = componenteRepository;
        _complejidadRepository = complejidadRepository;
    }

    public async Task<Guid> Handle(CreateRelacionCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el componente existe
        var componente = await _componenteRepository.GetByIdAsync(request.ComponenteId);
        if (componente == null)
        {
            throw new EntityNotFoundException("Componente", request.ComponenteId);
        }

        // Verificar que la complejidad existe
        var complejidad = await _complejidadRepository.GetByIdAsync(request.ComplejidadId);
        if (complejidad == null)
        {
            throw new EntityNotFoundException("Complejidad", request.ComplejidadId);
        }

        // Verificar si ya existe una relación para este par
        var existingRelacion = await _relacionRepository.GetByComponenteAndComplejidadAsync(request.ComponenteId, request.ComplejidadId);
        if (existingRelacion != null)
        {
            throw new EntityAlreadyExistsException("Relación", "componente-complejidad", $"{componente.Nombre}-{complejidad.Nombre}");
        }

        // Validar que las horas sean positivas
        if (request.Horas <= 0)
        {
            throw new ValidationException("Horas", "debe ser mayor a 0");
        }

        var relacion = new RelacionComponenteComplejidad
        {
            Id = Guid.NewGuid(),
            ComponenteId = request.ComponenteId,
            ComplejidadId = request.ComplejidadId,
            Horas = request.Horas
        };

        await _relacionRepository.CreateAsync(relacion);
        return relacion.Id;
    }
}
