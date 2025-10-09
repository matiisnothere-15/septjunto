using Microsoft.AspNetCore.Mvc;
using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SegurosSura.Evaluaciones.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
}