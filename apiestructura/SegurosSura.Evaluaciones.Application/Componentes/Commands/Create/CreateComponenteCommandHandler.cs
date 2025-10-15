using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;

public class CreateComponenteCommandHandler : IRequestHandler<CreateComponenteCommand, Guid>
{
    private readonly IComponenteRepository _componenteRepository;
    private readonly IProyectoRepository _proyectoRepository;

    public CreateComponenteCommandHandler(IComponenteRepository componenteRepository, IProyectoRepository proyectoRepository)
    {
        _componenteRepository = componenteRepository;
        _proyectoRepository = proyectoRepository;
    }

    public async Task<Guid> Handle(CreateComponenteCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya existe un componente con el mismo nombre
        var existingComponente = await _componenteRepository.GetByNameAsync(request.Nombre);
        if (existingComponente != null)
        {
            throw new EntityAlreadyExistsException("Componente", "nombre", request.Nombre);
        }

        // Determinar ProyectoId: usar el enviado o un proyecto existente por defecto
        Guid proyectoId;
        if (request.ProyectoId.HasValue)
        {
            proyectoId = request.ProyectoId.Value;
            var proyectoExists = await _proyectoRepository.GetByIdAsync(proyectoId) != null;
            if (!proyectoExists)
            {
                throw new EntityNotFoundException("Proyecto", proyectoId);
            }
        }
        else
        {
            // Si no se envía, tomar el primer proyecto disponible
            var proyectos = await _proyectoRepository.GetAllAsync();
            var first = proyectos.FirstOrDefault();
            if (first == null)
            {
                // Crear un proyecto por defecto si no existe ninguno
                var defaultProyecto = new Proyecto
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Proyecto por defecto",
                    Descripcion = "Generado automáticamente",
                    Fecha = DateTime.UtcNow,
                    HorasTotales = 0,
                    DiasEstimados = 0,
                    Riesgo = 0
                };
                var created = await _proyectoRepository.AddAsync(defaultProyecto);
                proyectoId = created.Id;
            }
            else
            {
                proyectoId = first.Id;
            }
        }

        var componente = new Componente
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = true,
            ProyectoId = proyectoId
        };

        await _componenteRepository.CreateAsync(componente);
        return componente.Id;
    }
}
