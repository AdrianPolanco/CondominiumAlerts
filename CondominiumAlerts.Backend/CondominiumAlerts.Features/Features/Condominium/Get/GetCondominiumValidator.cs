

using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominium.Get
{
    public class GetCondominiumValidator : AbstractValidator<GetCondominiumCommand>
    {
        public GetCondominiumValidator() {
            RuleFor(gc => gc.CondominiumId)
                .NotEmpty().WithMessage("The condominium was not especified");

            RuleFor(gc => gc.CondominiumId)
                .Must(id => Guid.TryParse(id, out _)).WithMessage("The id had an incorrect format");
        }

    }
}
