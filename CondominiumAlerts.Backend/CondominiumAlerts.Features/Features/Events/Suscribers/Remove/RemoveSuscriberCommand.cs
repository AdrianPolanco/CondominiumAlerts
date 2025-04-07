using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Suscribers.Remove;

public record RemoveSuscriberCommand(string UserId, Guid EventId): ICommand<Result<RemoveSuscriberResponse>>;