using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Features.Features.Messages;

namespace CondominiumAlerts.Features.Features.Events.Create;

public record CreateEventResponse(
    Guid Id, 
    string Title, 
    string Description, 
    DateTime Start, 
    DateTime End,
    bool IsStarted,
    bool IsFinished,
    bool IsToday,
    UserDto CreatedBy,
    List<UserDto> Suscribers,
    CondominiumDto Condominium,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );