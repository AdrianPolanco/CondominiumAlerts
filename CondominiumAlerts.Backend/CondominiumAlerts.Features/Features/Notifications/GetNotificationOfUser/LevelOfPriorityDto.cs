
namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public record LevelOfPriorityDto(
        Guid Id,
        string Title,
        int Priority
    );
}