using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Delete;

public class DeleteEvaluacionCommandHandler : IRequestHandler<DeleteEvaluacionCommand>
{
    private readonly IEvaluacionRepository _evaluacionRepository;

    public DeleteEvaluacionCommandHandler(IEvaluacionRepository evaluacionRepository)
    {
        _evaluacionRepository = evaluacionRepository;
    }

    public async Task Handle(DeleteEvaluacionCommand request, CancellationToken cancellationToken)
    {
        var evaluacion = await _evaluacionRepository.GetByIdAsync(request.Id);
        if (evaluacion == null)
        {
            throw new EntityNotFoundException("Evaluación", request.Id);
        }

        await _evaluacionRepository.DeleteAsync(request.Id);
    }
}
