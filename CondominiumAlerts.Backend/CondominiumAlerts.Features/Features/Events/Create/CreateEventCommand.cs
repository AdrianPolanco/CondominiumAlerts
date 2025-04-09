using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Create;

public record CreateEventCommand(
    string Title,
    string Description,
    DateTime Start,
    DateTime End,
    string CreatedById,
    Guid CondominiumId): ICommand<Result<CreateEventResponse>>;