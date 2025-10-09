using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Componentes.Queries.GetById;

public class GetComponenteByIdQueryHandler : IRequestHandler<GetComponenteByIdQuery, Componente?>
{
    private readonly IComponenteRepository _componenteRepository;

    public GetComponenteByIdQueryHandler(IComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }

    public async Task<Componente?> Handle(GetComponenteByIdQuery request, CancellationToken cancellationToken)
    {
        return await _componenteRepository.GetByIdAsync(request.Id);
    }
}
