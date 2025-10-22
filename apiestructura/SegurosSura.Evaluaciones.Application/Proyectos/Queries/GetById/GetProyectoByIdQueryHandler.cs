using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Queries.GetById;

public class GetProyectoByIdQueryHandler : IRequestHandler<GetProyectoByIdQuery, Proyecto?>
{
    private readonly IProyectoRepository _repository;

    public GetProyectoByIdQueryHandler(IProyectoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Proyecto?> Handle(GetProyectoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
