using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetById;

public class GetRelacionByIdQueryHandler : IRequestHandler<GetRelacionByIdQuery, RelacionComponenteComplejidad?>
{
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public GetRelacionByIdQueryHandler(IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _relacionRepository = relacionRepository;
    }

    public async Task<RelacionComponenteComplejidad?> Handle(GetRelacionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _relacionRepository.GetByIdAsync(request.Id);
    }
}
