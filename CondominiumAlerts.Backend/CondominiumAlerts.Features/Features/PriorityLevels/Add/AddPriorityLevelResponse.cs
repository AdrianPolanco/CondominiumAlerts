

using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Add
{
    public record AddPriorityLevelResponse(
           Guid Id,
           string Title,
           int Priority,
           string Description,
           Guid? CondominiumId,
           DateTime CreatedAt
        )
    {
        public static implicit operator AddPriorityLevelResponse(LevelOfPriority levelOfPriority) => new(
            levelOfPriority.Id,
            levelOfPriority.Title,
            levelOfPriority.Priority,
            levelOfPriority.Description,
            levelOfPriority.CondominiumId,
            levelOfPriority.CreatedAt
        );
    };

}
