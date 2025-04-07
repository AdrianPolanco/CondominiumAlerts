using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Delete;

public record DeleteEventCommand(Guid EventId, string CreatedById, string RequesterId): ICommand<Result<DeleteEventResponse>>;