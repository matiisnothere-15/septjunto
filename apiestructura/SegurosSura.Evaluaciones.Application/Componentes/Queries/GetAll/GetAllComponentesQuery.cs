using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Componentes.Queries.GetAll;

public record GetAllComponentesQuery : IRequest<IEnumerable<Componente>>;
