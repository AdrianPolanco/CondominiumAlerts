using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;
using LightResults;

namespace CondominiumAlerts.Features.Features.Users.Register;

public class RegisterUserCommand : ICommand<Result<RegisterUserResponse>>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public Phone PhoneNumber { get; set; } = new();
    
}