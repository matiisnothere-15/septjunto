using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Create;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Update;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplejidadesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ComplejidadesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Complejidad>>> GetAll()
    {
        var query = new GetAllComplejidadesQuery();
        var complejidades = await _mediator.Send(query);
        return Ok(complejidades);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Complejidad>> GetById(Guid id)
    {
        var query = new GetComplejidadByIdQuery(id);
        var complejidad = await _mediator.Send(query);
        
        if (complejidad == null)
        {
            return NotFound();
        }
        
        return Ok(complejidad);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateComplejidadCommand command)
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
            var command = new DeleteComplejidadCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
