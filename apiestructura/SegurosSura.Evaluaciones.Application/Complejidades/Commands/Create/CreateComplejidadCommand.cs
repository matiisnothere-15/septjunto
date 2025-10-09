using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Create;

public record CreateComplejidadCommand(
    string Nombre,
    int Orden
) : IRequest<Guid>;
