using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;
using SegurosSura.Evaluaciones.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

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
        // ----------------------------------------------------------------------
        // 1. SOLUCIÓN: Validar que el Proyecto EXISTA
        // ----------------------------------------------------------------------
        // Usamos el método GetByNameAsync que creamos y mejoramos.
        // Esto soluciona el error "Cannot insert the value NULL..."
        var proyecto = await _proyectoRepository.GetByNameAsync(request.NombreProyecto, cancellationToken);

        if (proyecto == null)
        {
            // Fallamos rápido y con un mensaje claro. No más fallbacks.
            // Esto le dirá al frontend (con un error 400) que el proyecto no existe.
            throw new ValidationException($"El proyecto con nombre '{request.NombreProyecto}' no fue encontrado en la base de datos.");
        }
        
        // Ahora tenemos un proyectoIdFinal garantizado.
        var proyectoIdFinal = proyecto.Id;

        // ----------------------------------------------------------------------
        // 2. Crear la entidad Evaluación
        // ----------------------------------------------------------------------
        var evaluacion = new Evaluacion
        {
            Id = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            NombreProyecto = request.NombreProyecto,
            DeltaRiesgoPct = request.DeltaRiesgoPct,
            
            // !! ESTA ES LA LÍNEA QUE ARREGLA EL ERROR !!
            // Asignamos el ID válido que encontramos.
            ProyectoId = proyectoIdFinal 
        };

        // ----------------------------------------------------------------------
        // 3. Validar y procesar detalles
        // ----------------------------------------------------------------------
        decimal horasTotales = 0;
        foreach (var detalleDto in request.Detalles)
        {
            // Validar que el componente existe
            var componente = await _componenteRepository.GetByIdAsync(detalleDto.ComponenteId, cancellationToken);
            if (componente == null) 
                throw new EntityNotFoundException("Componente", detalleDto.ComponenteId);

            // VALIDACIÓN ADICIONAL (RECOMENDADA):
            // Asegurarse que el componente pertenece al proyecto seleccionado.
            if (componente.ProyectoId != proyectoIdFinal)
            {
                throw new ValidationException($"El componente '{componente.Nombre}' no pertenece al proyecto '{proyecto.Nombre}'.");
            }

            // Validar que la complejidad existe
            var complejidad = await _complejidadRepository.GetByIdAsync(detalleDto.ComplejidadId, cancellationToken);
            if (complejidad == null) 
                throw new EntityNotFoundException("Complejidad", detalleDto.ComplejidadId);

            // Obtener las horas de la relación
            var relacion = await _relacionRepository.GetByComponenteAndComplejidadAsync(
                detalleDto.ComponenteId, detalleDto.ComplejidadId, cancellationToken);

            if (relacion == null)
            {
                // Damos un mensaje de error más claro, usando los nombres.
                throw new ValidationException($"No existe relación definida entre el componente '{componente.Nombre}' y la complejidad '{complejidad.Nombre}'.");
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
            
            // Asegúrate que la colección Detalles en tu entidad Evaluacion esté inicializada
            // (ej. public List<EvaluacionDetalle> Detalles { get; set; } = new List<EvaluacionDetalle>();)
            evaluacion.Detalles.Add(detalle);
            horasTotales += relacion.Horas;
        }

        // ----------------------------------------------------------------------
        // 4. Calcular totales
        // ----------------------------------------------------------------------
        
        // Asignar horas totales base
        evaluacion.HorasTotales = horasTotales; 

        // Calcular horas con riesgo
        if (request.DeltaRiesgoPct.HasValue && request.DeltaRiesgoPct != 0)
        {
            evaluacion.HorasTotalesConRiesgo = Math.Round(horasTotales * (1 + request.DeltaRiesgoPct.Value / 100m), 2);
        }
        else
        {
            evaluacion.HorasTotalesConRiesgo = horasTotales;
        }

        // ----------------------------------------------------------------------
        // 5. Guardar y retornar
        // ----------------------------------------------------------------------
        await _evaluacionRepository.CreateAsync(evaluacion, cancellationToken);

        return evaluacion.Id;
    }
}