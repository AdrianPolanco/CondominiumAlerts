namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public class NotificationDto
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string? Description { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required LevelOfPriorityDto? LevelOfPriority { get; set; }
        public required Guid? CondominiumId { get; set; }
        public required bool Read { get; set; }
    }
}