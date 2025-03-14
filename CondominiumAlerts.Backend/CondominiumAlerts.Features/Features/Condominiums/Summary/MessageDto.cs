using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Condominiums.Summary;

public record MessageDto(
    Guid Id,
    string Text,
    Guid CondominiumId,
    Condominium Condominium,
    Guid CreatorUserId,
    User CreatorUser,
    DateTime CreatedAt);
