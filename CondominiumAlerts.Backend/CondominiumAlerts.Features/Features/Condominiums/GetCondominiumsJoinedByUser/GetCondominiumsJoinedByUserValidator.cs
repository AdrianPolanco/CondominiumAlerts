

using FluentValidation;

namespace CondominiumAlerts.Features.Features.Condominiums.GetCondominiumsJoinedByUser
{
    public class GetCondominiumsJoinedByUserValidator : AbstractValidator<GetCondominiumsJoinedByUserCommand>
    {
        public GetCondominiumsJoinedByUserValidator()
        {
            RuleFor(gc => gc.UserId).NotEmpty().WithMessage("The user was not specified");
        }
    }
}
