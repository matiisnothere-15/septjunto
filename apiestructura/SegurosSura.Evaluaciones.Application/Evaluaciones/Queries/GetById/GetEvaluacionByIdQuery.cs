using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetById;

public record GetEvaluacionByIdQuery(Guid Id) : IRequest<Evaluacion?>;
