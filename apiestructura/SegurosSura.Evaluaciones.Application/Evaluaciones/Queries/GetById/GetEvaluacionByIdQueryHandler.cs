using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetById;

public class GetEvaluacionByIdQueryHandler : IRequestHandler<GetEvaluacionByIdQuery, Evaluacion?>
{
    private readonly IEvaluacionRepository _evaluacionRepository;

    public GetEvaluacionByIdQueryHandler(IEvaluacionRepository evaluacionRepository)
    {
        _evaluacionRepository = evaluacionRepository;
    }

    public async Task<Evaluacion?> Handle(GetEvaluacionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _evaluacionRepository.GetByIdAsync(request.Id);
    }
}
