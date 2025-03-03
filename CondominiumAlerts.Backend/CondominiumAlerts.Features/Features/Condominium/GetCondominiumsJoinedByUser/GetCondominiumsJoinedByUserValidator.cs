

using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominium.GetCondominiumsJoinedByUser
{
    public class GetCondominiumsJoinedByUserValidator : AbstractValidator<GetCondominiumsJoinedByUserCommand>
    {
        public GetCondominiumsJoinedByUserValidator()
        {
            RuleFor(gc => gc.UserId).NotEmpty().WithMessage("The user was not especified");
        }
    }
}
