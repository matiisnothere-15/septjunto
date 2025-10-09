using FluentValidation;
using SegurosSura.Evaluaciones.Application.Complejidades.Commands.Create;

namespace SegurosSura.Evaluaciones.Application.Complejidades.Commands.Create;

public class CreateComplejidadCommandValidator : AbstractValidator<CreateComplejidadCommand>
{
    public CreateComplejidadCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre de la complejidad es requerido")
            .MaximumLength(50)
            .WithMessage("El nombre no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-Z\s]+$")
            .WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.Orden)
            .GreaterThan(0)
            .WithMessage("El orden debe ser mayor a 0")
            .LessThanOrEqualTo(10)
            .WithMessage("El orden no puede ser mayor a 10");
    }
}
