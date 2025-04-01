
using FluentValidation;

namespace CondominiumAlerts.Features.Features.Users.GetCondominiumsUsers
{
    public class GetCondominiumsUsersValidator : AbstractValidator<GetCondominiumsUsersCommand>
    {
        public GetCondominiumsUsersValidator()
        {
            RuleFor(gc => gc.CondominiumId).NotEmpty().WithMessage("The condominium was not provided");

            RuleFor(gc => gc.CondominiumId).Must(c => Guid.TryParse(c, out _)).WithMessage("The condominium information was not in the desired format");

        }
    }
}
