using CondominiumAlerts.Features.Features.Messages;

namespace CondominiumAlerts.Features.Features.Events.Update;

public record UpdateEventResponse(
    Guid Id, 
    string Title, 
    string Description, 
    DateTime Start, 
    DateTime End,
    bool IsStarted,
    bool IsFinished,
    bool IsToday,
    DateTime CreatedAt,
    DateTime UpdatedAt);