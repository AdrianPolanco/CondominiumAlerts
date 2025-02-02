
using FluentValidation;
using PhoneNumbers;

namespace CondominiumAlerts.Features.Commands;

//RegisterUserCommand(string Username, string Email, string Password, string Name, string Lastname, Phone PhoneNumber, Address Address)
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
        RuleFor(u => u.Name).NotEmpty().WithMessage("El nombre de usuario es requerido.");
        RuleFor(u => u.Lastname).NotEmpty().WithMessage("La nombre de usuario es requerido.");
        RuleFor(u => u.Lastname).MinimumLength(3).MaximumLength(200).WithMessage("El apellido debe tener entre 3 y 200 caracteres.");
        RuleFor(u => u.PhoneNumber).NotNull().WithMessage("El phone number es requerido.");
        // Validación de PhoneNumber
        RuleFor(u => u.PhoneNumber)
            .NotNull().WithMessage("El número de teléfono es requerido."); // Verifica que PhoneNumber no sea null
        RuleFor(u => u.PhoneNumber.Number).Must(p => BeAValidPhoneNumber(p)).WithMessage("El número de teléfono debe ser válido.") // Verifica que el número sea válido
            .When(u => !string.IsNullOrEmpty(u.PhoneNumber?.Number)); // Solo verifica que sea válido si PhoneNumber existe
    }
    
    private bool BeAValidPhoneNumber(string phoneNumber)
    {
        try
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var number = phoneNumberUtil.Parse(phoneNumber, "ZZ"); // "ZZ" permite cualquier país
            return phoneNumberUtil.IsValidNumber(number);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
}