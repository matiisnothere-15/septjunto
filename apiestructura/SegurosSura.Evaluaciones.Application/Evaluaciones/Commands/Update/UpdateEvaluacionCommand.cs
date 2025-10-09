using MediatR;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Update;

public record EvaluacionDetalleDto(
    Guid ComponenteId,
    Guid ComplejidadId,
    string DescripcionTarea
);

public record UpdateEvaluacionCommand(
    Guid Id,
    string NombreProyecto,
    decimal? DeltaRiesgoPct,
    List<EvaluacionDetalleDto> Detalles
) : IRequest;
