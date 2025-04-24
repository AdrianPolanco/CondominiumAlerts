namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public class LevelOfPriorityDto
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required int Priority { get; set; }
    }
}