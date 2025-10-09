using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Entities;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Create;

public class CreateComplejidadCommandHandler : IRequestHandler<CreateComplejidadCommand, Guid>
{
    private readonly IComplejidadRepository _complejidadRepository;

    public CreateComplejidadCommandHandler(IComplejidadRepository complejidadRepository)
    {
        _complejidadRepository = complejidadRepository;
    }

    public async Task<Guid> Handle(CreateComplejidadCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya existe una complejidad con el mismo nombre
        var existingComplejidad = await _complejidadRepository.GetByNameAsync(request.Nombre);
        if (existingComplejidad != null)
        {
            throw new EntityAlreadyExistsException("Complejidad", "nombre", request.Nombre);
        }

        // Verificar si ya existe una complejidad con el mismo orden
        var existingOrden = await _complejidadRepository.ExistsByOrderAsync(request.Orden);
        if (existingOrden)
        {
            throw new EntityAlreadyExistsException("Complejidad", "orden", request.Orden.ToString());
        }

        var complejidad = new Complejidad
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Orden = request.Orden,
            Activo = true
        };

        await _complejidadRepository.CreateAsync(complejidad);
        return complejidad.Id;
    }
}
