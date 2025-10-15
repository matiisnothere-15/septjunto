using FluentValidation;
using SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;

namespace SegurosSura.Evaluaciones.Application.Componentes.Commands.Create;

public class CreateComponenteCommandValidator : AbstractValidator<CreateComponenteCommand>
{
    public CreateComponenteCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del componente es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres");

        RuleFor(x => x.ProyectoId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("ProyectoId, si se envía, debe ser un GUID válido");
    }
}
