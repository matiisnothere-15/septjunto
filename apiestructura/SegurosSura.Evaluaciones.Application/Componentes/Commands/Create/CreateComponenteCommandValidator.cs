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
            .WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-Z0-9\s\-_]+$")
            .WithMessage("El nombre solo puede contener letras, números, espacios, guiones y guiones bajos");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres");
    }
}
