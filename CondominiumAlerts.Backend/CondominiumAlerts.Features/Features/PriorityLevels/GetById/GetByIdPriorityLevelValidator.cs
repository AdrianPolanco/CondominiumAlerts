using FluentValidation;


namespace CondominiumAlerts.Features.Features.PriorityLevels.GetById
{
    public class GetByIdPriorityLevelValidator : AbstractValidator<GetByIdPriorityLevelQuery>
    {
        public GetByIdPriorityLevelValidator()
        {
            RuleFor(l => l.CondominiumId)
             .NotEmpty().WithMessage("The condominium was not especified");

            RuleFor(l => l.Id)
             .NotEmpty().WithMessage("The id was not especified");
        }
    }
}
