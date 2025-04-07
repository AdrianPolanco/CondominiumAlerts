using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Suscribers.Add;

public record AddSuscriberCommand(string UserId, Guid EventId): ICommand<Result<AddSuscriberResponse>>;