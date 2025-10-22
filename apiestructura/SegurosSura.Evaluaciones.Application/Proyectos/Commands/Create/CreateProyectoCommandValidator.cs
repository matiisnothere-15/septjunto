using FluentValidation;

namespace SegurosSura.Evaluaciones.Application.Proyectos.Commands.Create;

public class CreateProyectoCommandValidator : AbstractValidator<CreateProyectoCommand>
{
    public CreateProyectoCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del proyecto es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(1000).WithMessage("La descripción no puede exceder 1000 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Descripcion));
    }
}
