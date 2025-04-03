
using FluentValidation;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Update
{
    public class UpdatePriorityLevelValidator : AbstractValidator<UpdatePriorityLevelCommand>
    {
        public UpdatePriorityLevelValidator()
        {
            RuleFor(l => l.Id)
                .NotEmpty().WithMessage("The id was not especified");

            RuleFor(l => l.Title)
               .NotEmpty().WithMessage("The title was not especified");

            RuleFor(l => l.Description)
                .NotEmpty().WithMessage("The description was not especified");

            RuleFor(l => l.Priority)
                .GreaterThanOrEqualTo(1).WithMessage("The priority provided is invalid, the priority must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("The priority provided is invalid, the priority must be at most 10");


        }
    }
}
