using CondominiumAlerts.Features.Features.Messages;

namespace CondominiumAlerts.Features.Features.Events.Suscribers.Add;

public record AddSuscriberResponse(bool Added, Guid EventId, UserDto AddedUser, DateTime AddedAt);