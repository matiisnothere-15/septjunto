using FluentValidation;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create;

public class CreateProyectoCommandValidator : AbstractValidator<CreateProyectoCommand>
{
    public CreateProyectoCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proyecto es requerido")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(2000).WithMessage("La descripción no puede exceder 2000 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));

        RuleFor(x => x.DiasEstimados)
            .GreaterThanOrEqualTo(0).WithMessage("Los días estimados no pueden ser negativos")
            .When(x => x.DiasEstimados.HasValue);

        RuleFor(x => x.HorasTotales)
            .GreaterThanOrEqualTo(0).WithMessage("Las horas totales no pueden ser negativas")
            .When(x => x.HorasTotales.HasValue);

        RuleFor(x => x.Riesgo)
            .InclusiveBetween(0, 999.99m).WithMessage("Riesgo fuera de rango")
            .When(x => x.Riesgo.HasValue);
    }
}
