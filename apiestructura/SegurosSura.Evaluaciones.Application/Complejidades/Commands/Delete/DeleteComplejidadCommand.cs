using MediatR;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Delete;

public record DeleteComplejidadCommand(Guid Id) : IRequest;
