using MediatR;
using SegurosSura.Evaluaciones.Domain.Entities;
using System.Collections.Generic;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Queries.GetAll;

public class GetAllProyectosQuery : IRequest<IEnumerable<Proyecto>>
{
}