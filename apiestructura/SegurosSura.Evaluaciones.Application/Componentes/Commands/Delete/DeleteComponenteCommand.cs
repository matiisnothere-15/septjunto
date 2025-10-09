using MediatR;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Delete;

public record DeleteComponenteCommand(Guid Id) : IRequest;
