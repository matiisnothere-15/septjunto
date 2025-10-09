using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SegurosSura.Evaluaciones.Application.Interfaces;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Queries.GetAll;

public class GetAllProyectosQueryHandler : IRequestHandler<GetAllProyectosQuery, IEnumerable<Proyecto>>
{
    private readonly IProyectoRepository _repository;

    public GetAllProyectosQueryHandler(IProyectoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Proyecto>> Handle(GetAllProyectosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}