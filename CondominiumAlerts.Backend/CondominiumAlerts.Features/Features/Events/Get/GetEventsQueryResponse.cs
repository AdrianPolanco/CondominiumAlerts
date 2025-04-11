using CondominiumAlerts.Features.Features.Messages;

namespace CondominiumAlerts.Features.Features.Events.Get;

public record GetEventsQueryResponse(
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
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsSuscribed);