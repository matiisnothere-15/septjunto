using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Update;

public class UpdateEvaluacionCommandHandler : IRequestHandler<UpdateEvaluacionCommand>
{
    private readonly IEvaluacionRepository _evaluacionRepository;
    private readonly IComponenteRepository _componenteRepository;
    private readonly IComplejidadRepository _complejidadRepository;
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public UpdateEvaluacionCommandHandler(
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

    public async Task Handle(UpdateEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await _evaluacionRepository.GetByIdAsync(request.Id);
        if (evaluacion == null)
        {
            throw new EntityNotFoundException("Evaluación", request.Id);
        }

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

        // Actualizar propiedades básicas
        evaluacion.NombreProyecto = request.NombreProyecto;
        evaluacion.DeltaRiesgoPct = request.DeltaRiesgoPct;

        // Limpiar detalles existentes
        evaluacion.Detalles.Clear();

        // Crear nuevos detalles y calcular horas
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
        else
        {
            evaluacion.HorasTotalesConRiesgo = null;
        }

        await _evaluacionRepository.UpdateAsync(evaluacion);
    }
}
