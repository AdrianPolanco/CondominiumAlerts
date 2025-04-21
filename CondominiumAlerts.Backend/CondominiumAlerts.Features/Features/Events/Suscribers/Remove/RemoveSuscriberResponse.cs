using CondominiumAlerts.Features.Features.Messages;

namespace CondominiumAlerts.Features.Features.Events.Suscribers.Remove;

public record RemoveSuscriberResponse(bool Removed, string EventTitle, Guid EventId, UserDto RemovedUser, DateTime RemovedAt);