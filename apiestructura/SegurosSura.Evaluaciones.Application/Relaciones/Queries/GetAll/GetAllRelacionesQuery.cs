using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetAll;

public record GetAllRelacionesQuery : IRequest<IEnumerable<RelacionComponenteComplejidad>>;
