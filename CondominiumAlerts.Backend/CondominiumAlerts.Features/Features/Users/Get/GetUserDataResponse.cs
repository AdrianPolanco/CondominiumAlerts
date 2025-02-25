using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Features.Features.Users;

public record GetUserDataResponse(string Id, string Name, string Lastname, string Username, string Email, string ProfilePictureUrl, Address? Address, DateTime CreatedAt);