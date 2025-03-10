using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;
using LightResults;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Users.Update;

public record UpdateUserCommand(string Id, string Username, string Name, string Lastname, IFormFile? ProfilePic, Address Address) : ICommand<Result<UpdateUserResponse>>;