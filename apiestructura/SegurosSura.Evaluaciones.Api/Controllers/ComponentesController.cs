using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Update;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Componentes.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Componentes.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Api.Dtos;

namespace SegurosSura.Evaluaciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponentesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ComponentesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComponenteDto>>> GetAll()
    {
        var query = new GetAllComponentesQuery();
        var componentes = await _mediator.Send(query);
        var dtos = componentes.Select(c => new ComponenteDto(c.Id, c.Nombre, c.Descripcion, c.Activo, c.ProyectoId));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComponenteDto>> GetById(Guid id)
    {
        var query = new GetComponenteByIdQuery(id);
        var componente = await _mediator.Send(query);
        
        if (componente == null)
        {
            return NotFound();
        }
        var dto = new ComponenteDto(componente.Id, componente.Nombre, componente.Descripcion, componente.Activo, componente.ProyectoId);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateComponenteCommand command)
    {
        var componenteId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = componenteId }, componenteId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateComponenteCommand command)
    {
        var updateCommand = command with { Id = id };
        await _mediator.Send(updateCommand);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteComponenteCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
