
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Add
{
    public class AddPriorityLevelValidator : AbstractValidator<AddPriorityLevelCommand>
    {

        public AddPriorityLevelValidator()
        {
            RuleFor(l => l.Title)
                .NotEmpty().WithMessage("The title was not especified");

            RuleFor(l => l.Description)
                .NotEmpty().WithMessage("The description was not especified");

            RuleFor(l => l.Priority)
                .GreaterThanOrEqualTo(1).WithMessage("The priority provided is invalid, the priority must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("The priority provided is invalid, the priority must be at most 10"); 

            RuleFor(l => l.CondominiumId)
                .NotEmpty().WithMessage("The condominium was not especified");

        }
    }
}
