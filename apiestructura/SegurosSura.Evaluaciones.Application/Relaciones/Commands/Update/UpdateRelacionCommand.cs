using MediatR;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Update;

public record UpdateRelacionCommand(
    Guid Id,
    decimal Horas
) : IRequest;
