using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Update;

public record UpdateEventCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    string CreatedById,
    Guid CondominiumId,
    string EditorId): ICommand<Result<UpdateEventResponse>>;