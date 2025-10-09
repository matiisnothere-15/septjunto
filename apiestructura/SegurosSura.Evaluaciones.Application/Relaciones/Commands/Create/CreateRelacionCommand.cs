using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Create;

public record CreateRelacionCommand(
    Guid ComponenteId,
    Guid ComplejidadId,
    decimal Horas
) : IRequest<Guid>;
