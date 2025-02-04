using CondominiumAlerts.Domain.Events.Interfaces;

namespace CondominiumAlerts.Features.Features.Users.Register;

public record UserRegisteredEvent(string Id, string Name, string LastName, string Email, DateTime Date) : IEvent
{
    public UserRegisteredEvent(string Id, string Name, string LastName, string Email)
        : this(Id, Name, LastName, Email, DateTime.UtcNow)
    { }
}