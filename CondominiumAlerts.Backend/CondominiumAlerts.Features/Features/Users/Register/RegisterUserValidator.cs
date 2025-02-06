using FluentValidation;
using PhoneNumbers;

namespace CondominiumAlerts.Features.Features.Users.Register;

//RegisterUserCommand(string Username, string Email, string Password, string Name, string Lastname, Phone PhoneNumber, Address Address)
public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(u => u.Email).NotEmpty().NotNull().WithMessage("El email es requerido.");         
        RuleFor(u => u.Email).EmailAddress().WithMessage("Debe escribir un email válido.");         
        RuleFor(u => u.Username).NotEmpty().NotNull().WithMessage("El username es requerido.");         
        RuleFor(u => u.Username).MinimumLength(4).MaximumLength(25).WithMessage("El nombre de usuario debe tener entre 4 y 25 caracteres.");         
        RuleFor(u => u.Password).NotEmpty().WithMessage("El password es requerido.");         
        RuleFor(u => u.Password).MinimumLength(8).MaximumLength(250)             
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número.")
            .Matches(@"[\W_]").WithMessage("La contraseña debe contener al menos un carácter especial.")
            .Matches(@"^[a-zA-Z0-9\W_]+$").WithMessage("La contraseña solo puede contener caracteres alfanuméricos y especiales.");    
        //RuleFor(u => u.ConfirmPassword).NotEmpty().WithMessage("El password confirmation es requerido.");
        //RuleFor(u => u.ConfirmPassword).Equal(u => u.Password).WithMessage("Las contraseñas no coinciden.");
        RuleFor(u => u.Name).NotEmpty().WithMessage("El nombre es requerido.");         
        RuleFor(u => u.Lastname).NotEmpty().WithMessage("El apellido es requerido.");         
        RuleFor(u => u.Lastname).MinimumLength(3).MaximumLength(200).WithMessage("El apellido debe tener entre 3 y 200 caracteres.");         

        // Validación de PhoneNumber en una sola regla
        RuleFor(u => u.PhoneNumber)
            .NotNull().WithMessage("El teléfono es requerido")
            .DependentRules(() => {
                RuleFor(u => u.PhoneNumber.Number)
                    .NotEmpty().WithMessage("El número de teléfono no puede estar vacío")
                    .Must(BeAValidPhoneNumber)
                    .WithMessage("El formato del número de teléfono no es válido. Debe incluir el código de país (ejemplo: +34123456789)");
            });
    }
    
    private bool BeAValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        try         
        {             
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();             
            var number = phoneNumberUtil.Parse(phoneNumber, "ZZ");
            return phoneNumberUtil.IsValidNumber(number);         
        }         
        catch (NumberParseException)         
        {             
            return false;         
        }     
    }
}