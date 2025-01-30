using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Commands;

public record RegisterUserCommand(string Username, string Email, string Password): ICommand<Result<object>>;