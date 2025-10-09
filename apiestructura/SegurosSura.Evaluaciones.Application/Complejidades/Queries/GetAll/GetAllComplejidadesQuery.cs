using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetAll;

public record GetAllComplejidadesQuery : IRequest<IEnumerable<Complejidad>>;
