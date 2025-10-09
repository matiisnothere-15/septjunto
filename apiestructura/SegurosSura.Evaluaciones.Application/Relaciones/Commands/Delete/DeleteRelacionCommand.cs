using MediatR;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Delete;

public record DeleteRelacionCommand(Guid Id) : IRequest;
