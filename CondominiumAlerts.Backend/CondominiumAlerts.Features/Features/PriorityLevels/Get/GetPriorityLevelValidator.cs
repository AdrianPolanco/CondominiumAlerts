
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Get
{
    public class GetPriorityLevelValidator : AbstractValidator<GetPriorityLevelsQuery>
    {
        public GetPriorityLevelValidator()
        {

           RuleFor(l => l.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("The page provided is invalid, the page number must be greater than 0");

            RuleFor(l => l.PageSize)
             .GreaterThanOrEqualTo(1).WithMessage("The page size provided is invalid, the page size must be greater than 0");         
            
            RuleFor(l => l.CondominiumId)
                .NotEmpty().WithMessage("The condominium was not especified");
        
        }
    }
}
