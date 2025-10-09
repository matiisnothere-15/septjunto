using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Delete;

public class DeleteComplejidadCommandHandler : IRequestHandler<DeleteComplejidadCommand>
{
    private readonly IComplejidadRepository _complejidadRepository;
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public DeleteComplejidadCommandHandler(
        IComplejidadRepository complejidadRepository,
        IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _complejidadRepository = complejidadRepository;
        _relacionRepository = relacionRepository;
    }

    public async Task Handle(DeleteComplejidadCommand request, CancellationToken cancellationToken)
    {
        var complejidad = await _complejidadRepository.GetByIdAsync(request.Id);
        if (complejidad == null)
        {
            throw new EntityNotFoundException("Complejidad", request.Id);
        }

        // Verificar si la complejidad tiene relaciones
        var relaciones = await _relacionRepository.GetByComplejidadAsync(request.Id);
        if (relaciones.Any())
        {
            throw new ValidationException("No se puede eliminar la complejidad porque tiene relaciones asociadas");
        }

        await _complejidadRepository.DeleteAsync(request.Id);
    }
}
