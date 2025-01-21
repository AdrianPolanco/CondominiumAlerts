using CondominiumAlerts.Domain.Events.Interfaces;

namespace CondominiumAlerts.Domain.Events;

public class UserRegisteredEvent : IEvent
{
    public Guid UserId { get; set; }
    public DateTime Date { get; init; }
}