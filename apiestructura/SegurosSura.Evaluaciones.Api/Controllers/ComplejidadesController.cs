using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Create;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Update;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Delete;
using SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetAll;
using SegurosSura.Evaluaciones.Application.Complejidades.Queries.GetById;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Api.Dtos;

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
    public async Task<ActionResult<IEnumerable<ComplejidadDto>>> GetAll()
    {
        var query = new GetAllComplejidadesQuery();
        var complejidades = await _mediator.Send(query);
        var dtos = complejidades.Select(c => new ComplejidadDto(c.Id, c.Nombre, c.Orden, c.Activo));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComplejidadDto>> GetById(Guid id)
    {
        var query = new GetComplejidadByIdQuery(id);
        var complejidad = await _mediator.Send(query);
        
        if (complejidad == null)
        {
            return NotFound();
        }
        var dto = new ComplejidadDto(complejidad.Id, complejidad.Nombre, complejidad.Orden, complejidad.Activo);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateComplejidadCommand command)
    {
        var complejidadId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = complejidadId }, complejidadId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateComplejidadCommand command)
    {
        var updateCommand = command with { Id = id };
        await _mediator.Send(updateCommand);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteComplejidadCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
