
using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.PriorityLevels.GetById
{
    public record GetByIdPriorityLevelResponse(
           Guid Id,
           string Title,
           int Priority,
           string Description,
           Guid? CondominiumId,
           DateTime CreatedAt
        )
    {
        public static implicit operator GetByIdPriorityLevelResponse(LevelOfPriority levelOfPriority) => new(
            levelOfPriority.Id,
            levelOfPriority.Title,
            levelOfPriority.Priority,
            levelOfPriority.Description,
            levelOfPriority.CondominiumId,
            levelOfPriority.CreatedAt
        );
    };
}
