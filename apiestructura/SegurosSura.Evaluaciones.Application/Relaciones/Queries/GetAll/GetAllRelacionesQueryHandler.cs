using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetAll;

public class GetAllRelacionesQueryHandler : IRequestHandler<GetAllRelacionesQuery, IEnumerable<RelacionComponenteComplejidad>>
{
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public GetAllRelacionesQueryHandler(IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _relacionRepository = relacionRepository;
    }

    public async Task<IEnumerable<RelacionComponenteComplejidad>> Handle(GetAllRelacionesQuery request, CancellationToken cancellationToken)
    {
        return await _relacionRepository.GetAllAsync();
    }
}
