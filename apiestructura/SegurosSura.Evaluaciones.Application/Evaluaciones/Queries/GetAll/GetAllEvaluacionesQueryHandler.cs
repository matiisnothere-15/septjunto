using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetAll;

public class GetAllEvaluacionesQueryHandler : IRequestHandler<GetAllEvaluacionesQuery, IEnumerable<Evaluacion>>
{
    private readonly IEvaluacionRepository _evaluacionRepository;

    public GetAllEvaluacionesQueryHandler(IEvaluacionRepository evaluacionRepository)
    {
        _evaluacionRepository = evaluacionRepository;
    }

    public async Task<IEnumerable<Evaluacion>> Handle(GetAllEvaluacionesQuery request, CancellationToken cancellationToken)
    {
        return await _evaluacionRepository.GetAllAsync();
    }
}
