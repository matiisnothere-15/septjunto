using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;

public class CreateEvaluacionCommandHandler : IRequestHandler<CreateEvaluacionCommand, Guid>
{
    private readonly IEvaluacionRepository _evaluacionRepository;
    private readonly IComponenteRepository _componenteRepository;
    private readonly IComplejidadRepository _complejidadRepository;
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;
    private readonly IProyectoRepository _proyectoRepository;

    public CreateEvaluacionCommandHandler(
        IEvaluacionRepository evaluacionRepository,
        IComponenteRepository componenteRepository,
        IComplejidadRepository complejidadRepository,
        IRelacionComponenteComplejidadRepository relacionRepository,
        IProyectoRepository proyectoRepository)
    {
        _evaluacionRepository = evaluacionRepository;
        _componenteRepository = componenteRepository;
        _complejidadRepository = complejidadRepository;
        _relacionRepository = relacionRepository;
        _proyectoRepository = proyectoRepository;
    }

    public async Task<Guid> Handle(CreateEvaluacionCommand request, CancellationToken cancellationToken)
    {
        // Intentar determinar ProyectoId a partir del nombre enviado (si existe)
        var proyecto = await _proyectoRepository.GetByNameAsync(request.NombreProyecto);

        // También determinaremos ProyectoId desde los componentes de los detalles (fuente más confiable)
        Guid? proyectoIdDesdeDetalles = null;

        // Validar que todos los componentes y complejidades existen (esto ya estaba bien)
        foreach (var detalle in request.Detalles)
        {
            var componente = await _componenteRepository.GetByIdAsync(detalle.ComponenteId);
            if (componente == null) throw new EntityNotFoundException("Componente", detalle.ComponenteId);

            // Capturar ProyectoId del primer componente y validar consistencia
            if (!proyectoIdDesdeDetalles.HasValue)
            {
                proyectoIdDesdeDetalles = componente.ProyectoId;
            }
            else if (proyectoIdDesdeDetalles.Value != componente.ProyectoId)
            {
                throw new ValidationException("Todos los componentes de la evaluación deben pertenecer al mismo proyecto.");
            }

            var complejidad = await _complejidadRepository.GetByIdAsync(detalle.ComplejidadId);
            if (complejidad == null) throw new EntityNotFoundException("Complejidad", detalle.ComplejidadId);
        }

        // Crear la evaluación
        // Resolver ProyectoId final: priorizar el del componente (consistencia de datos)
        var proyectoIdFinal = proyectoIdDesdeDetalles ?? proyecto?.Id;
        if (!proyectoIdFinal.HasValue)
        {
            // Fallback: si no hay nombre válido y no hay componentes válidos
            var todos = await _proyectoRepository.GetAllAsync();
            var primero = todos.FirstOrDefault();
            if (primero == null)
            {
                throw new ValidationException("No hay proyectos disponibles en la base de datos. Cree uno antes de registrar una evaluación.");
            }
            proyectoIdFinal = primero.Id;
        }

        var evaluacion = new Evaluacion
        {
            Id = Guid.NewGuid(),
            Fecha = DateTime.UtcNow, // O la fecha apropiada
            NombreProyecto = request.NombreProyecto,
            DeltaRiesgoPct = request.DeltaRiesgoPct,
            ProyectoId = proyectoIdFinal.Value
        };

        // Crear los detalles y calcular horas (esto ya estaba bien)
        decimal horasTotales = 0;
        foreach (var detalleDto in request.Detalles)
        {
            var relacion = await _relacionRepository.GetByComponenteAndComplejidadAsync(
                detalleDto.ComponenteId, detalleDto.ComplejidadId);

            if (relacion == null)
            {
                throw new ValidationException($"No existe relación definida entre componente {detalleDto.ComponenteId} y complejidad {detalleDto.ComplejidadId}. Configurela primero.");
            }

            var detalle = new EvaluacionDetalle
            {
                Id = Guid.NewGuid(),
                EvaluacionId = evaluacion.Id, // EF Core debería manejar esto
                ComponenteId = detalleDto.ComponenteId,
                ComplejidadId = detalleDto.ComplejidadId,
                HorasBase = relacion.Horas,
                DescripcionTarea = detalleDto.DescripcionTarea
            };

            // ¡IMPORTANTE! Asegúrate que la colección Detalles en tu entidad Evaluacion esté inicializada
            // (ej. public List<EvaluacionDetalle> Detalles { get; set; } = new List<EvaluacionDetalle>();)
            // Si no está inicializada, la siguiente línea dará NullReferenceException
            evaluacion.Detalles.Add(detalle);
            horasTotales += relacion.Horas;
        }

    evaluacion.HorasTotales = horasTotales; // Asignar horas totales base (propiedad ignorada por el mapeo si no existe en BD)

        // Calcular horas con riesgo (esto ya estaba bien)
        if (request.DeltaRiesgoPct.HasValue && request.DeltaRiesgoPct != 0)
        {
            evaluacion.HorasTotalesConRiesgo = Math.Round(horasTotales * (1 + request.DeltaRiesgoPct.Value / 100m), 2);
        }
        else
        {
            evaluacion.HorasTotalesConRiesgo = horasTotales;
        }

        // Guardar la evaluación (línea 89 aprox.)
        await _evaluacionRepository.CreateAsync(evaluacion);

        return evaluacion.Id;
    }
}