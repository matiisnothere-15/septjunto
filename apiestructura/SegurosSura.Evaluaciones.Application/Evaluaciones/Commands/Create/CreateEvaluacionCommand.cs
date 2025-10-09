using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;

public record EvaluacionDetalleDto(
    Guid ComponenteId,
    Guid ComplejidadId,
    string DescripcionTarea
);

public record CreateEvaluacionCommand(
    string NombreProyecto,
    decimal? DeltaRiesgoPct,
    List<EvaluacionDetalleDto> Detalles
) : IRequest<Guid>;
