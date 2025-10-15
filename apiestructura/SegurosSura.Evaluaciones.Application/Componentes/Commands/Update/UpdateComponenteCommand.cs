using MediatR;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Update;

public record UpdateComponenteCommand(
    Guid Id,
    string Nombre,
    string Descripcion = "",
    Guid? ProyectoId = null
) : IRequest;
