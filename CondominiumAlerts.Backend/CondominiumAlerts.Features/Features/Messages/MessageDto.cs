namespace CondominiumAlerts.Features.Features.Messages;

public record MessageDto(
    Guid Id,
    string Text,
    UserDto CreatorUser,
    string? CreatorUserId,
    string? ReceiverUserId,
    string? MediaUrl,
    Guid? CondominiumId,
    Guid? MessageBeingRepliedToId,
    DateTime CreatedAt
 );

