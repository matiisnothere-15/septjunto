using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetAll;

public class GetAllComplejidadesQueryHandler : IRequestHandler<GetAllComplejidadesQuery, IEnumerable<Complejidad>>
{
    private readonly IComplejidadRepository _complejidadRepository;

    public GetAllComplejidadesQueryHandler(IComplejidadRepository complejidadRepository)
    {
        _complejidadRepository = complejidadRepository;
    }

    public async Task<IEnumerable<Complejidad>> Handle(GetAllComplejidadesQuery request, CancellationToken cancellationToken)
    {
        return await _complejidadRepository.GetAllAsync();
    }
}
