using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominiums.Add
{
    public class AddCondominiumValidator
    : AbstractValidator<AddCondominiumCommand>
    {
        public AddCondominiumValidator()
        {
            RuleFor(c => c.userId).NotEmpty().WithMessage("No se especifico el usuario creador");
            RuleFor(c => c.Name).NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(c => c.Address).NotEmpty().WithMessage("La direccion es requerida.");
            RuleFor(c => c.ImageFile).NotNull().WithMessage("La imagen de condominio es requerida.");

        }
    }
}