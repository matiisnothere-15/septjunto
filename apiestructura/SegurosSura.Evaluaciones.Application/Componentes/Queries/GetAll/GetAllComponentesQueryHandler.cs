using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Componentes.Queries.GetAll;

public class GetAllComponentesQueryHandler : IRequestHandler<GetAllComponentesQuery, IEnumerable<Componente>>
{
    private readonly IComponenteRepository _componenteRepository;

    public GetAllComponentesQueryHandler(IComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }

    public async Task<IEnumerable<Componente>> Handle(GetAllComponentesQuery request, CancellationToken cancellationToken)
    {
        return await _componenteRepository.GetAllAsync();
    }
}
