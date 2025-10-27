using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create;
using SegurosSura.Evaluaciones.Application.Proyectos.Queries.GetById;

namespace SegurosSura.Evaluaciones.Api.Controllers;

[ApiController]
[Route("api/proyectos")]
public class ProyectosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProyectosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Proyecto>>> GetAll()
    {
        var query = new SegurosSura.Evaluaciones.Application.Proyectos.Queries.GetAll.GetAllProyectosQuery();
        var proyectos = await _mediator.Send(query);
        return Ok(proyectos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Proyecto>> GetById(Guid id)
    {
        var proyecto = await _mediator.Send(new GetProyectoByIdQuery(id));
        if (proyecto is null) return NotFound();
        return Ok(proyecto);
    }

    public record CreateProyectoRequest(
        string Nombre,
        string? Descripcion,
        int? DiasEstimados,
        DateTime? Fecha,
        decimal? HorasTotales,
        decimal? Riesgo
    );

    [HttpPost]
    public async Task<ActionResult<Proyecto>> Create([FromBody] CreateProyectoRequest body)
    {
        var command = new CreateProyectoCommand(
            body.Nombre,
            body.Descripcion,
            body.DiasEstimados,
            body.Fecha,
            body.HorasTotales,
            body.Riesgo
        );
        var created = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}