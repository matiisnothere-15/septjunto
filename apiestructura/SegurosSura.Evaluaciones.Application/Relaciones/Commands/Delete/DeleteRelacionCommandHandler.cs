using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Delete;

public class DeleteRelacionCommandHandler : IRequestHandler<DeleteRelacionCommand>
{
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public DeleteRelacionCommandHandler(IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _relacionRepository = relacionRepository;
    }

    public async Task Handle(DeleteRelacionCommand request, CancellationToken cancellationToken)
    {
        var relacion = await _relacionRepository.GetByIdAsync(request.Id);
        if (relacion == null)
        {
            throw new EntityNotFoundException("Relación", request.Id);
        }

        await _relacionRepository.DeleteAsync(request.Id);
    }
}
