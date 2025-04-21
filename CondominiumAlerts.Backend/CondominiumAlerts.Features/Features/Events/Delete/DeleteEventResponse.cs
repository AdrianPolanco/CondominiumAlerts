namespace CondominiumAlerts.Features.Features.Events.Delete;

public record DeleteEventResponse(
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