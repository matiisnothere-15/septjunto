using System;
using System.Collections.Generic;

namespace SegurosSura.Evaluaciones.Api.Dtos;

public record ComponenteDto(Guid Id, string Nombre, string Descripcion, bool Activo, Guid ProyectoId);
public record ComplejidadDto(Guid Id, string Nombre, int Orden, bool Activo);
public record RelacionDto(Guid Id, Guid ComponenteId, Guid ComplejidadId, decimal Horas);

public record EvaluacionDetalleDto(
    Guid Id,
    Guid ComponenteId,
    string ComponenteNombre,
    Guid ComplejidadId,
    string ComplejidadNombre,
    decimal HorasBase,
    string DescripcionTarea
);

public record EvaluacionDto(
    Guid Id,
    DateTime Fecha,
    string NombreProyecto,
    decimal? DeltaRiesgoPct,
    decimal HorasTotales,
    int DiasEstimados,
    decimal? HorasTotalesConRiesgo,
    IReadOnlyList<EvaluacionDetalleDto> Detalles
);
