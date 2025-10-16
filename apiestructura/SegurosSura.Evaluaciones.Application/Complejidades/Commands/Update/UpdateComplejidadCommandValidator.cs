using FluentValidation;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Update;

public class UpdateComplejidadCommandValidator : AbstractValidator<UpdateComplejidadCommand>
{
    public UpdateComplejidadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El Id de la complejidad es requerido");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la complejidad es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.Orden)
            .GreaterThan(0).WithMessage("El orden debe ser mayor a 0")
            .LessThanOrEqualTo(10).WithMessage("El orden no puede ser mayor a 10");
    }
}
