namespace CondominiumAlerts.Domain.Aggregates.ValueObjects;

using Domain.Aggregates.ValueObjects;
using FluentValidation;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(a => a.Street)
            .NotEmpty().WithMessage("La calle es obligatoria.")
            .MaximumLength(100).WithMessage("La calle no puede superar los 100 caracteres.");

        RuleFor(a => a.City)
            .NotEmpty().WithMessage("La ciudad es obligatoria.")
            .MaximumLength(50).WithMessage("La ciudad no puede superar los 50 caracteres.");

        RuleFor(a => a.PostalCode)
            .NotEmpty().WithMessage("El código postal es obligatorio.")
            .Matches(@"^\d{4,6}$").WithMessage("El código postal debe tener entre 4 y 6 dígitos.");
    }
}