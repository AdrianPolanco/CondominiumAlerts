using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;

public record MessageDto(
    Guid Id,
    string Text,
    Guid CondominiumId,
    Condominium Condominium,
    string CreatorUserId,
    User CreatorUser,
    DateTime CreatedAt);