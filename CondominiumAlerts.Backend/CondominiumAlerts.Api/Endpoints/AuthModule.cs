using System.Security.Claims;
using Carter;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.ValueObjects;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Users;
using CondominiumAlerts.Features.Features.Users.Register;
using CondominiumAlerts.Features.Features.Users.Update;
using Mapster;
using MediatR;

namespace CondominiumAlerts.Api.Endpoints;

public class AuthModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{Id}",
            async (string id, ISender sender, ClaimsPrincipal claims, CancellationToken cancellationToken) =>
            {
                var currentUserId = claims.FindFirst("user_id")?.Value;

                if (currentUserId != id || string.IsNullOrWhiteSpace(currentUserId.Trim()))
                    return Results.BadRequest(new
                        { IsSuccess = false, Message = "El Id del usuario no coincide con tus credenciales." });

                var queryRequest = new GetUserDataQuery(id);
                
                var result = await sender.Send(queryRequest, cancellationToken);

                var response = new
                {
                    IsSuccess = true,
                    Data = result.Value.Adapt<GetUserDataResponse>()
                };
                
                return Results.Ok(response);

            }).RequireAuthorization();
        
        app.MapPut("/users/edit", async (ClaimsPrincipal claims, HttpContext context, ISender sender, CancellationToken cancellationToken) =>
        {
            var currentUserId = claims.FindFirst("user_id")?.Value;
            
            if (currentUserId == null) return Results.BadRequest("El Id del usuario no existe.");
            
            var form = await context.Request.ReadFormAsync(); // Leer FormData

            // Extraer datos del formulario
            var id = form["Id"];
            var username = form["username"];
            var name = form["name"];
            var lastname = form["lastname"];
            var street = form["address.street"];
            var city = form["address.city"];
            var postalCode = form["address.postalCode"];
            // Obtener archivo
            var profilePic = form.Files.GetFile("profilePic");
            
            var address = new Address(street, city, postalCode);
            
            if (currentUserId != id || string.IsNullOrWhiteSpace(currentUserId.Trim()))
                return Results.BadRequest(new
                    { IsSuccess = false, Message = "El Id del usuario no coincide con tus credenciales." });
            
            var updateUserCommand = new UpdateUserCommand(currentUserId, username, name, lastname, profilePic, address);

            var result = await sender.Send(updateUserCommand, cancellationToken);
            if(result.IsFailed) return Results.BadRequest(result);

            var response = new
            {
                IsSuccess = result.IsSuccess,
                Data = result.Value.Adapt<UpdateUserResponse>()
            };
            
            return Results.Ok(response);
        }).RequireAuthorization();
        
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

        app.MapPost("/users/register/google/{Id}",
            async (string id, RegisterUserCommand command, CancellationToken cancellationToken, ClaimsPrincipal claims, IRepository<User, string> repository) =>
            {
                var currentUserId = claims.FindFirst("user_id")?.Value;
                
                if (currentUserId == null) return Results.BadRequest("El Id del usuario no existe.");
                
                if (currentUserId != id || string.IsNullOrWhiteSpace(currentUserId.Trim()))
                    return Results.BadRequest(new
                        { IsSuccess = false, Message = "El Id del usuario no coincide con tus credenciales." });
                
                var user = command.Adapt<User>();
                user.Id = id;
                
                user = await repository.CreateAsync(user, cancellationToken);

                var response = new
                {
                    IsSuccess = true,
                    Data = user.Adapt<RegisterUserResponse>()
                };
                
                return Results.Ok(response);
            }).RequireAuthorization();

        app.MapGet("/users/exists/{id}",
            async (string id, IRepository<User, string> userRepository, CancellationToken cancellationToken) =>
            {
                var user = await userRepository.GetByIdAsync(id, cancellationToken, readOnly: true);

                var response = new
                {
                    DoesUserExist = user is not null
                };

                return Results.Ok(response);
            });
    }
}