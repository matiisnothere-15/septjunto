using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create;

public class CreateProyectoCommandHandler : IRequestHandler<CreateProyectoCommand, Proyecto>
{
    private readonly IProyectoRepository _proyectoRepository;

    public CreateProyectoCommandHandler(IProyectoRepository proyectoRepository)
    {
        _proyectoRepository = proyectoRepository;
    }

    public async Task<Proyecto> Handle(CreateProyectoCommand request, CancellationToken cancellationToken)
    {
        // Normaliza y valida nombre
        var nombreTrimmed = request.Nombre.Trim();
        var existing = await _proyectoRepository.GetByNameAsync(nombreTrimmed, cancellationToken);
        if (existing != null)
        {
            // Para Create, lanzamos validación si ya existe
            throw new ValidationException($"Ya existe un proyecto con el nombre '{nombreTrimmed}'.");
        }

        // Solo establecemos los campos que son intrínsecos al comando
        var proyecto = new Proyecto
        {
            Id = Guid.NewGuid(),
            Nombre = nombreTrimmed,
            Descripcion = request.Descripcion?.Trim() ?? string.Empty
        };

        // Si llegan valores opcionales, los respetamos; si no, dejamos que la BD aplique defaults
        if (request.DiasEstimados.HasValue) proyecto.DiasEstimados = request.DiasEstimados.Value;
        if (request.HorasTotales.HasValue) proyecto.HorasTotales = request.HorasTotales.Value;
        if (request.Riesgo.HasValue) proyecto.Riesgo = request.Riesgo.Value;
        if (request.Fecha.HasValue) proyecto.Fecha = request.Fecha.Value.Date;

        return await _proyectoRepository.AddAsync(proyecto, cancellationToken);
    }
}
