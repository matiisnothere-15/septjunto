using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Delete;

public class DeleteComponenteCommandHandler : IRequestHandler<DeleteComponenteCommand>
{
    private readonly IComponenteRepository _componenteRepository;
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public DeleteComponenteCommandHandler(
        IComponenteRepository componenteRepository,
        IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _componenteRepository = componenteRepository;
        _relacionRepository = relacionRepository;
    }

    public async Task Handle(DeleteComponenteCommand request, CancellationToken cancellationToken)
    {
        var componente = await _componenteRepository.GetByIdAsync(request.Id);
        if (componente == null)
        {
            throw new EntityNotFoundException("Componente", request.Id);
        }

        // Verificar si el componente tiene relaciones
        var relaciones = await _relacionRepository.GetByComponenteAsync(request.Id);
        if (relaciones.Any())
        {
            throw new ValidationException("No se puede eliminar el componente porque tiene relaciones asociadas");
        }

        await _componenteRepository.DeleteAsync(request.Id);
    }
}
