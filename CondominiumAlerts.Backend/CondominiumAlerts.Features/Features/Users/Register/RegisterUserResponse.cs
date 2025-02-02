using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Features.Commands;

public record RegisterUserResponse(string Id, string Name, string LastName, string Email, DateTime CreatedAt, Phone Phone);