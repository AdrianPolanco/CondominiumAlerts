using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Get
{
    public record GetPriorityLevelResponce(int PageNumber,int PageSize,int TotalRecords,List<PriorityDto> Priorities);


    public record PriorityDto(Guid Id,
    string Title,
    string Description,
    int Priority,
    Guid? CondominiumId,
    DateTime CreatedAt)
    {
        public static implicit operator PriorityDto(LevelOfPriority levelOfPriority) => new(
            levelOfPriority.Id,
            levelOfPriority.Title,
            levelOfPriority.Description,
            levelOfPriority.Priority,
            levelOfPriority.CondominiumId,
            levelOfPriority.CreatedAt
        );
    };
}
