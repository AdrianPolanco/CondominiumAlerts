using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Features.Features.Users.Register;

public record RegisterUserResponse(string Id, string Name, string Lastname, string Username, string Email, DateTime CreatedAt);