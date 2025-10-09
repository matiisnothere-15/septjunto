using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Create;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Update;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Relaciones.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;

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
    public async Task<ActionResult<IEnumerable<RelacionComponenteComplejidad>>> GetAll()
    {
        var query = new GetAllRelacionesQuery();
        var relaciones = await _mediator.Send(query);
        return Ok(relaciones);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RelacionComponenteComplejidad>> GetById(Guid id)
    {
        var query = new GetRelacionByIdQuery(id);
        var relacion = await _mediator.Send(query);
        
        if (relacion == null)
        {
            return NotFound();
        }
        
        return Ok(relacion);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRelacionCommand command)
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
            var command = new DeleteRelacionCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
