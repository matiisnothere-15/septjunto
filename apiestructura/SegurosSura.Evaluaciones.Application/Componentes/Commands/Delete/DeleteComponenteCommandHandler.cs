using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Delete;

public class DeleteComponenteCommandHandler : IRequestHandler<DeleteComponenteCommand>
{
    private readonly IComponenteRepository _componenteRepository;
    public DeleteComponenteCommandHandler(IComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }

    public async Task Handle(DeleteComponenteCommand request, CancellationToken cancellationToken)
    {
        var componente = await _componenteRepository.GetByIdAsync(request.Id);
        if (componente == null)
        {
            throw new EntityNotFoundException("Componente", request.Id);
        }

        // Eliminar físicamente. Las relaciones dependientes se eliminan por cascade
        // de acuerdo con la configuración del DbContext.
        await _componenteRepository.DeleteAsync(request.Id);
    }
}
