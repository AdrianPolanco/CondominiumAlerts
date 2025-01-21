using CondominiumAlerts.Domain.Events.Interfaces;

namespace CondominiumAlerts.Features.Events.Users;

public class CreateUserEvent : IEvent
{
    public DateTime Date { get; }
}