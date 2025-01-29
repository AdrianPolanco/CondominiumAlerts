using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.CrossCutting.Results;

namespace CondominiumAlerts.Features.Commands;

public record RegisterUserCommand(string Username, string Email, string Password): ICommand<CustomResult<object>>;