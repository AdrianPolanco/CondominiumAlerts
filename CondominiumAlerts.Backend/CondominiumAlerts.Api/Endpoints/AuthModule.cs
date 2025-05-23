﻿using Carter;
using CondominiumAlerts.CrossCutting.Results;
using CondominiumAlerts.Features.Commands;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.Api.Endpoints;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register",
            async (ISender sender, RegisterUserCommand registerUserCommand, CancellationToken cancellationToken) =>
            {
               var result = await sender.Send(registerUserCommand, cancellationToken);
               if(result.IsFailed) return Results.BadRequest(result);
               var response = new
               {
                   IsSuccess = result.IsSuccess,
                   Data = result.Value.Adapt<RegisterUserResponse>()
               };
               return Results.Ok(response);
            });
    }
}