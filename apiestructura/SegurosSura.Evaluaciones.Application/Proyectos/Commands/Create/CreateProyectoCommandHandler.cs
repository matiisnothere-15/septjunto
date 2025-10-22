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
        var existing = await _proyectoRepository.GetByNameAsync(request.Nombre);
        if (existing != null)
        {
            throw new EntityAlreadyExistsException("Proyecto", "Nombre", request.Nombre);
        }

        var proyecto = new Proyecto
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre.Trim(),
            Descripcion = request.Descripcion?.Trim() ?? string.Empty,
            Fecha = DateTime.UtcNow.Date,
            HorasTotales = 0m,
            DiasEstimados = 0,
            Riesgo = 0m
        };

        return await _proyectoRepository.AddAsync(proyecto);
    }
}
