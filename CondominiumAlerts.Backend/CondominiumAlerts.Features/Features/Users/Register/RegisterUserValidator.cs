
using FluentValidation;

namespace CondominiumAlerts.Features.Commands;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(u => u.Email).NotEmpty().WithMessage("El email es requerido.");
        RuleFor(u => u.Email).EmailAddress().WithMessage("Debe escribir un email válido.");
        RuleFor(u => u.Username).NotEmpty().NotNull().WithMessage("El username es requerido.");
        RuleFor(u => u.Username).MinimumLength(4).MaximumLength(25).WithMessage("El nombre de usuario debe tener entre 4 y 25 caracteres.");
        RuleFor(u => u.Password).NotEmpty().WithMessage("El password es requerido.");
        RuleFor(u => u.Password).MinimumLength(8).MaximumLength(250)
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.") // Al menos un número
            .Matches(@"[\W_]").WithMessage("La contraseña debe contener al menos un carácter especial.") // Al menos un carácter especial
            .Matches(@"^[a-zA-Z0-9\W_]+$").WithMessage("La contraseña solo puede contener caracteres alfanuméricos y especiales.");
    }
}