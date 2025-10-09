using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetAll;

public record GetAllEvaluacionesQuery : IRequest<IEnumerable<Evaluacion>>;
