using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Update;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Componentes.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Componentes.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;

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
    public async Task<ActionResult<IEnumerable<Componente>>> GetAll()
    {
        var query = new GetAllComponentesQuery();
        var componentes = await _mediator.Send(query);
        return Ok(componentes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Componente>> GetById(Guid id)
    {
        var query = new GetComponenteByIdQuery(id);
        var componente = await _mediator.Send(query);
        
        if (componente == null)
        {
            return NotFound();
        }
        
        return Ok(componente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateComponenteCommand command)
    {
        try
        {
            var updateCommand = command with { Id = id };
            await _mediator.Send(updateCommand);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteComponenteCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
