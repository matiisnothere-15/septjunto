using MediatR;
using SegurosSura.Evaluaciones.Application.Interfaces;
using SegurosSura.Evaluaciones.Domain.Exceptions;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Update;

public class UpdateRelacionCommandHandler : IRequestHandler<UpdateRelacionCommand>
{
    private readonly IRelacionComponenteComplejidadRepository _relacionRepository;

    public UpdateRelacionCommandHandler(IRelacionComponenteComplejidadRepository relacionRepository)
    {
        _relacionRepository = relacionRepository;
    }

    public async Task Handle(UpdateRelacionCommand request, CancellationToken cancellationToken)
    {
        var relacion = await _relacionRepository.GetByIdAsync(request.Id);
        if (relacion == null)
        {
            throw new EntityNotFoundException("Relación", request.Id);
        }

        // Validar que las horas sean positivas
        if (request.Horas <= 0)
        {
            throw new ValidationException("Horas", "debe ser mayor a 0");
        }

        relacion.Horas = request.Horas;
        await _relacionRepository.UpdateAsync(relacion);
    }
}
