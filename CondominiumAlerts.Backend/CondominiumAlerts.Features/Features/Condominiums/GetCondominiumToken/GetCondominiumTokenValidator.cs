using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominiums.GenerateLink;

public class GetCondominiumTokenValidator : AbstractValidator<GetCondominiumTokenCommand>
{
    public GetCondominiumTokenValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("The user was not specified User");
        
        RuleFor(c => c.CondominiumId)
            .NotEmpty()
            .WithMessage("The condominium was not specified");
    }
}