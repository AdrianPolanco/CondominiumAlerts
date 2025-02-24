using CondominiumAlerts.Domain.Aggregates.ValueObjects;

namespace CondominiumAlerts.Features.Features.Users.Update;

public class UpdateUserResponse
{
    public string Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public Address? Address { get; set; }
}