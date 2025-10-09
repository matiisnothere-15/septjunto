using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;

public class CreateEvaluacionCommandHandler : IRequestHandler<CreateEvaluacionCommand, Guid>
{
    private readonly IEvaluacionRepository _evaluacionRepository;
    private readonly IComponenteRepository _componenteRepository;
    private readonly IComplejidadRepository _complejidadRepository;
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public CreateEvaluacionCommandHandler(
        IEvaluacionRepository evaluacionRepository,
        IComponenteRepository componenteRepository,
        IComplejidadRepository complejidadRepository,
        IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _evaluacionRepository = evaluacionRepository;
        _componenteRepository = componenteRepository;
        _complejidadRepository = complejidadRepository;
        _relacionRepository = relacionRepository;
    }

    public async Task<Guid> Handle(CreateEvaluacionCommand request, CancellationToken cancellationToken)
    {
        // Validar que todos los componentes y complejidades existen
        foreach (var detalle in request.Detalles)
        {
            var componente = await _componenteRepository.GetByIdAsync(detalle.ComponenteId);
            if (componente == null)
            {
                throw new EntityNotFoundException("Componente", detalle.ComponenteId);
            }

            var complejidad = await _complejidadRepository.GetByIdAsync(detalle.ComplejidadId);
            if (complejidad == null)
            {
                throw new EntityNotFoundException("Complejidad", detalle.ComplejidadId);
            }
        }

        // Crear la evaluación
        var evaluacion = new Evaluacion
        {
            Id = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            NombreProyecto = request.NombreProyecto,
            DeltaRiesgoPct = request.DeltaRiesgoPct
        };

        // Crear los detalles y calcular horas
        decimal horasTotales = 0;
        foreach (var detalleDto in request.Detalles)
        {
            // Obtener las horas base de la relación
            var relacion = await _relacionRepository.GetByComponenteAndComplejidadAsync(
                detalleDto.ComponenteId, detalleDto.ComplejidadId);
            
            if (relacion == null)
            {
                throw new ValidationException($"No existe relación entre componente {detalleDto.ComponenteId} y complejidad {detalleDto.ComplejidadId}");
            }

            var detalle = new EvaluacionDetalle
            {
                Id = Guid.NewGuid(),
                EvaluacionId = evaluacion.Id,
                ComponenteId = detalleDto.ComponenteId,
                ComplejidadId = detalleDto.ComplejidadId,
                HorasBase = relacion.Horas,
                DescripcionTarea = detalleDto.DescripcionTarea
            };

            evaluacion.Detalles.Add(detalle);
            horasTotales += relacion.Horas;
        }

        evaluacion.HorasTotales = horasTotales;

        // Calcular horas con riesgo si se especifica
        if (request.DeltaRiesgoPct.HasValue && request.DeltaRiesgoPct > 0)
        {
            evaluacion.HorasTotalesConRiesgo = Math.Round(horasTotales * (1 + request.DeltaRiesgoPct.Value / 100), 2);
        }

        await _evaluacionRepository.CreateAsync(evaluacion);
        return evaluacion.Id;
    }
}
