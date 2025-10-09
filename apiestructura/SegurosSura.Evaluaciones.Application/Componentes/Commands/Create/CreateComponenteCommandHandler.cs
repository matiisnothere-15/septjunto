using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;

public class CreateComponenteCommandHandler : IRequestHandler<CreateComponenteCommand, Guid>
{
    private readonly IComponenteRepository _componenteRepository;

    public CreateComponenteCommandHandler(IComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }

    public async Task<Guid> Handle(CreateComponenteCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya existe un componente con el mismo nombre
        var existingComponente = await _componenteRepository.GetByNameAsync(request.Nombre);
        if (existingComponente != null)
        {
            throw new EntityAlreadyExistsException("Componente", "nombre", request.Nombre);
        }

        var componente = new Componente
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = true
        };

        await _componenteRepository.CreateAsync(componente);
        return componente.Id;
    }
}
