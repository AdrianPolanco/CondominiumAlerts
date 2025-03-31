namespace CondominiumAlerts.Features.Features.Messages;

public record MessageDto(
    Guid Id,
    string Text,
    string? MediaUrl,
    Guid? CondominiumId,
    string? ReceiverUserId,
    Guid? MessageBeingRepliedToId,
    DateTime CreatedAt
 );

