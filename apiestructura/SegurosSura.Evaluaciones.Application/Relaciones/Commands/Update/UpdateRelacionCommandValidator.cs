using FluentValidation;

namespace SegurosSura.Evaluaciones.Application.Relaciones.Commands.Update;

public class UpdateRelacionCommandValidator : AbstractValidator<UpdateRelacionCommand>
{
    public UpdateRelacionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id de la relación es requerido");

        RuleFor(x => x.Horas)
            .GreaterThan(0).WithMessage("Las horas deben ser mayor a 0")
            .LessThanOrEqualTo(1000).WithMessage("Las horas no pueden exceder 1000")
            .PrecisionScale(8, 2, false).WithMessage("Las horas pueden tener máximo 2 decimales y 8 dígitos en total");
    }
}
