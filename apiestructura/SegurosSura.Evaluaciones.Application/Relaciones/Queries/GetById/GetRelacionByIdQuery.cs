using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetById;

public record GetRelacionByIdQuery(Guid Id) : IRequest<RelacionComponenteComplejidad?>;
