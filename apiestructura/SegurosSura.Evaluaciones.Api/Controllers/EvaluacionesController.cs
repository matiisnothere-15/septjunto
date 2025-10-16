using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Update;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Api.Dtos;
using ApiEvaluacionDto = SegurosSura.Evaluaciones.Api.Dtos.EvaluacionDto;
using ApiEvaluacionDetalleDto = SegurosSura.Evaluaciones.Api.Dtos.EvaluacionDetalleDto;

namespace SegurosSura.Evaluaciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EvaluacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApiEvaluacionDto>>> GetAll()
    {
        var query = new GetAllEvaluacionesQuery();
        var evaluaciones = await _mediator.Send(query);
        var dtos = evaluaciones.Select(e =>
        {
            var detalles = e.Detalles.Select(d => new ApiEvaluacionDetalleDto(
                d.Id,
                d.ComponenteId,
                d.Componente?.Nombre ?? string.Empty,
                d.ComplejidadId,
                d.Complejidad?.Nombre ?? string.Empty,
                d.HorasBase,
                d.DescripcionTarea
            )).ToList();

            var horasTotales = detalles.Sum(d => d.HorasBase);
            var horasConRiesgo = e.HorasTotalesConRiesgo ?? horasTotales;
            var diasEstimados = (int)Math.Ceiling(horasConRiesgo / 6m);

            return new ApiEvaluacionDto(
                e.Id,
                e.Fecha,
                e.NombreProyecto,
                e.DeltaRiesgoPct,
                horasTotales,
                diasEstimados,
                e.HorasTotalesConRiesgo,
                detalles
            );
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiEvaluacionDto>> GetById(Guid id)
    {
        var query = new GetEvaluacionByIdQuery(id);
        var evaluacion = await _mediator.Send(query);
        if (evaluacion == null)
        {
            return NotFound();
        }
        var detalles = evaluacion.Detalles.Select(d => new ApiEvaluacionDetalleDto(
            d.Id,
            d.ComponenteId,
            d.Componente?.Nombre ?? string.Empty,
            d.ComplejidadId,
            d.Complejidad?.Nombre ?? string.Empty,
            d.HorasBase,
            d.DescripcionTarea
        )).ToList();
        var horasTotales = detalles.Sum(d => d.HorasBase);
        var horasConRiesgo = evaluacion.HorasTotalesConRiesgo ?? horasTotales;
        var diasEstimados = (int)Math.Ceiling(horasConRiesgo / 6m);

        var dto = new ApiEvaluacionDto(
            evaluacion.Id,
            evaluacion.Fecha,
            evaluacion.NombreProyecto,
            evaluacion.DeltaRiesgoPct,
            horasTotales,
            diasEstimados,
            evaluacion.HorasTotalesConRiesgo,
            detalles
        );
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateEvaluacionCommand command)
    {
        var evaluacionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = evaluacionId }, evaluacionId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEvaluacionCommand command)
    {
        var updateCommand = command with { Id = id };
        await _mediator.Send(updateCommand);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteEvaluacionCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
