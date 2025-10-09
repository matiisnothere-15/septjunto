using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Update;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;

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
    public async Task<ActionResult<IEnumerable<Evaluacion>>> GetAll()
    {
        var query = new GetAllEvaluacionesQuery();
        var evaluaciones = await _mediator.Send(query);
        return Ok(evaluaciones);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Evaluacion>> GetById(Guid id)
    {
        var query = new GetEvaluacionByIdQuery(id);
        var evaluacion = await _mediator.Send(query);
        
        if (evaluacion == null)
        {
            return NotFound();
        }
        
        return Ok(evaluacion);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateEvaluacionCommand command)
    {
        try
        {
            var evaluacionId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = evaluacionId }, evaluacionId);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEvaluacionCommand command)
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
            var command = new DeleteEvaluacionCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
