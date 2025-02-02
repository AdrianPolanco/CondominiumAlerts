using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;
using LightResults;

namespace CondominiumAlerts.Features.Commands;

public record RegisterUserCommand(string Username, string Email, string Password, string Name, string Lastname, Phone PhoneNumber, Address Address): ICommand<Result<object>>;