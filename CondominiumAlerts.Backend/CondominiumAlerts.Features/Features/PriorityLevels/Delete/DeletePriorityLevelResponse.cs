
using CondominiumAlerts.Domain.Aggregates.Entities;
namespace CondominiumAlerts.Features.Features.PriorityLevels.Delete
{
    public record DeletePriorityLevelResponse(
           Guid Id,
           string Title,
           int Priority,
           string Description,
           Guid? CondominiumId,
           DateTime UpdatedAt
        )
    {
        public static implicit operator DeletePriorityLevelResponse(LevelOfPriority levelOfPriority) => new(
            levelOfPriority.Id,
            levelOfPriority.Title,
            levelOfPriority.Priority,
            levelOfPriority.Description,
            levelOfPriority.CondominiumId,
            levelOfPriority.UpdatedAt
        );
    };
}
