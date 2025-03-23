
using FluentValidation;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Get
{
    public class GetPriorityLevelValidator : AbstractValidator<GetPriorityLevelsQuery>
    {
        public GetPriorityLevelValidator()
        {
            RuleFor(l => l.CondominiumId)
                .NotEmpty().WithMessage("The condominium was not especified");

           RuleFor(l => l.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("The page provided is invalid, the page number must be greater than 0");

            RuleFor(l => l.PageSize)
             .GreaterThanOrEqualTo(1).WithMessage("The page size provided is invalid, the page size must be greater than 0");
        }
    }
}
