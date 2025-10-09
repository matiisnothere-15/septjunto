using MediatR;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Update;

public record UpdateComplejidadCommand(
    Guid Id,
    string Nombre,
    int Orden
) : IRequest;
