namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public record class NotificationDto(
        Guid Id,
        string Title,
        string? Description,
        DateTime CreatedAt,
        LevelOfPriorityDto LevelOfPriority
    );
}

