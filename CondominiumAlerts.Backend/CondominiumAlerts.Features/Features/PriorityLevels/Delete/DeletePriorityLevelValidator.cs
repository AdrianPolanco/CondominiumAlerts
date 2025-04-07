
using FluentValidation;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Delete
{
    public class DeletePriorityLevelValidator : AbstractValidator<DeletePriorityLevelCommand>
    {
        public DeletePriorityLevelValidator()
        {

            RuleFor(l => l.CondominiumId)
                .NotEmpty().WithMessage("The condominium was not especified");

            RuleFor(l => l.Id)
             .NotEmpty().WithMessage("The id was not especified");
        }
    }
}
