

using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Update
{
    public record UpdatePriorityLevelResponse(
           Guid Id,
           string Title,
           int Priority,
           string Description,
           Guid? CondominiumId,
           DateTime UpdatedAt
        )
    {
        public static implicit operator UpdatePriorityLevelResponse(LevelOfPriority levelOfPriority) => new(
            levelOfPriority.Id,
            levelOfPriority.Title,
            levelOfPriority.Priority,
            levelOfPriority.Description,
            levelOfPriority.CondominiumId,
            levelOfPriority.UpdatedAt
        );
    };
}
