using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetById;

public record GetComplejidadByIdQuery(Guid Id) : IRequest<Complejidad?>;
