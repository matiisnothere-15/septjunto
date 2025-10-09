using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Componentes.Queries.GetById;

public record GetComponenteByIdQuery(Guid Id) : IRequest<Componente?>;
