using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Create;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Update;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Api.Dtos;

namespace SegurosSura.Evaluaciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RelacionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RelacionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelacionDto>>> GetAll()
    {
        var query = new GetAllRelacionesQuery();
        var relaciones = await _mediator.Send(query);
        var dtos = relaciones.Select(r => new RelacionDto(r.Id, r.ComponenteId, r.ComplejidadId, r.Horas));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RelacionDto>> GetById(Guid id)
    {
        var query = new GetRelacionByIdQuery(id);
        var relacion = await _mediator.Send(query);
        
        if (relacion == null)
        {
            return NotFound();
        }
        var dto = new RelacionDto(relacion.Id, relacion.ComponenteId, relacion.ComplejidadId, relacion.Horas);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateRelacionCommand command)
    {
        var relacionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = relacionId }, new { id = relacionId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRelacionCommand command)
    {
        var updateCommand = command with { Id = id };
        await _mediator.Send(updateCommand);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteRelacionCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
