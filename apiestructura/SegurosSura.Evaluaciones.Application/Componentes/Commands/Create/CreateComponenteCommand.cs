using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;

public record CreateComponenteCommand(
    string Nombre,
    string Descripcion = "",
    Guid? ProyectoId = null
) : IRequest<Guid>;
