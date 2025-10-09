using FluentValidation;
using SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;

namespace SegurosSura.Evaluaciones.Application.Evaluaciones.Commands.Create;

public class EvaluacionDetalleDtoValidator : AbstractValidator<EvaluacionDetalleDto>
{
    public EvaluacionDetalleDtoValidator()
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

        RuleFor(x => x.DescripcionTarea)
            .NotEmpty()
            .WithMessage("La descripción de la tarea es requerida")
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres")
            .MinimumLength(10)
            .WithMessage("La descripción debe tener al menos 10 caracteres");
    }
}

public class CreateEvaluacionCommandValidator : AbstractValidator<CreateEvaluacionCommand>
{
    public CreateEvaluacionCommandValidator()
    {
        RuleFor(x => x.NombreProyecto)
            .NotEmpty()
            .WithMessage("El nombre del proyecto es requerido")
            .MaximumLength(200)
            .WithMessage("El nombre del proyecto no puede exceder 200 caracteres")
            .MinimumLength(3)
            .WithMessage("El nombre del proyecto debe tener al menos 3 caracteres");

        RuleFor(x => x.DeltaRiesgoPct)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El porcentaje de riesgo no puede ser negativo")
            .LessThanOrEqualTo(100)
            .WithMessage("El porcentaje de riesgo no puede exceder 100%")
            .When(x => x.DeltaRiesgoPct.HasValue);

        RuleFor(x => x.Detalles)
            .NotEmpty()
            .WithMessage("Debe incluir al menos un detalle de evaluación")
            .Must(detalles => detalles.Count >= 1)
            .WithMessage("Debe incluir al menos un detalle de evaluación")
            .Must(detalles => detalles.Count <= 50)
            .WithMessage("No puede incluir más de 50 detalles de evaluación");

        RuleForEach(x => x.Detalles)
            .SetValidator(new EvaluacionDetalleDtoValidator());
    }
}
