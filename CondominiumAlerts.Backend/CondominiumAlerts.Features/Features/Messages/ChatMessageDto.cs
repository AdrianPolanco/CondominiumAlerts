using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Messages;

public record ChatMessageDto(
    Guid Id,
    string Text,
    Guid? CondominiumId,
    string? ReceiverUserId,
    Guid? MessageBeingRepliedToId,
    DateTime CreatedAt,
    UserDto CreatorUser,
    string? MediaUrl);
    