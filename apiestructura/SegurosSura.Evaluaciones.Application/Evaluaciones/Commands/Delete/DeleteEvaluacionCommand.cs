using MediatR;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Delete;

public record DeleteEvaluacionCommand(Guid Id) : IRequest;
