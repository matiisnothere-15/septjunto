using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetById;

public class GetComplejidadByIdQueryHandler : IRequestHandler<GetComplejidadByIdQuery, Complejidad?>
{
    private readonly IComplejidadRepository _complejidadRepository;

    public GetComplejidadByIdQueryHandler(IComplejidadRepository complejidadRepository)
    {
        _complejidadRepository = complejidadRepository;
    }

    public async Task<Complejidad?> Handle(GetComplejidadByIdQuery request, CancellationToken cancellationToken)
    {
        return await _complejidadRepository.GetByIdAsync(request.Id);
    }
}
