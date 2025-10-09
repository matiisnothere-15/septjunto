using FluentValidation;
using SegurosSura.Evaluaciones.Application.Relaciones.Commands.Create;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Create;

public class CreateRelacionCommandValidator : AbstractValidator<CreateRelacionCommand>
{
    public CreateRelacionCommandValidator()
    {
        RuleFor(x => x.ComponenteId)
            .NotEmpty()
            .WithMessage("El ID del componente es requerido")
            .NotEqual(Guid.Empty)
            .WithMessage("El ID del componente no puede ser vacío");

        RuleFor(x => x.ComplejidadId)
            .NotEmpty()
            .WithMessage("El ID de la complejidad es requerido")
            .NotEqual(Guid.Empty)
            .WithMessage("El ID de la complejidad no puede ser vacío");

        RuleFor(x => x.Horas)
            .GreaterThan(0)
            .WithMessage("Las horas deben ser mayor a 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Las horas no pueden exceder 1000")
            .PrecisionScale(8, 2, false)
            .WithMessage("Las horas pueden tener máximo 2 decimales y 8 dígitos en total");
    }
}
