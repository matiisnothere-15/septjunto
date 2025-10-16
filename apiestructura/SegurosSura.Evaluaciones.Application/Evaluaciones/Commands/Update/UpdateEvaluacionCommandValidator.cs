using FluentValidation;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Update;

public class UpdateEvaluacionCommandValidator : AbstractValidator<UpdateEvaluacionCommand>
{
    public UpdateEvaluacionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id de la evaluación es requerido");

        RuleFor(x => x.NombreProyecto)
            .NotEmpty().WithMessage("El nombre del proyecto es requerido")
            .MaximumLength(200).WithMessage("El nombre del proyecto no puede exceder 200 caracteres")
            .MinimumLength(3).WithMessage("El nombre del proyecto debe tener al menos 3 caracteres");

        RuleFor(x => x.DeltaRiesgoPct)
            .GreaterThanOrEqualTo(0).WithMessage("El porcentaje de riesgo no puede ser negativo")
            .LessThanOrEqualTo(100).WithMessage("El porcentaje de riesgo no puede exceder 100%")
            .When(x => x.DeltaRiesgoPct.HasValue);
    }
}
