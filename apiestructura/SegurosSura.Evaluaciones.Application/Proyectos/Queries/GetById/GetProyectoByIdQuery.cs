using System;
using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Queries.GetById;

public record GetProyectoByIdQuery(Guid Id) : IRequest<Proyecto?>;
