using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create;

public record CreateProyectoCommand(
    string Nombre,
    string? Descripcion
) : IRequest<Proyecto>;
