using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create;

public record CreateProyectoCommand(
    string Nombre,
    string? Descripcion,
    int? DiasEstimados = null,
    DateTime? Fecha = null,
    decimal? HorasTotales = null,
    decimal? Riesgo = null
) : IRequest<Proyecto>;
