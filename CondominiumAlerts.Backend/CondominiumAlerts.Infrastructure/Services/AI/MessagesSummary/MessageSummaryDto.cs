using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;

public record MessageSummaryDto(
    Guid Id,
    string Text,
    Guid CondominiumId,
    Condominium Condominium,
    string CreatorUserId,
    User CreatorUser,
    string CreatorUsername,
    string CreatorUserFullname,
    DateTime CreatedAt);