using Carter;
using CondominiumAlerts.Features.Features.Posts.Create;
using CondominiumAlerts.Features.Features.Posts.Delete;
using CondominiumAlerts.Features.Features.Posts.Get;
using CondominiumAlerts.Features.Features.Posts.Update;
using LightResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.Api.Endpoints
{
    public class PostsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // Endpoint para obtener todos los posts de un condominio
            app.MapGet("/posts", async (ISender sender, [FromQuery] Guid condominiumId, CancellationToken cancellationToken) =>
            {
                var command = new GetPostsCommand { CondominiumId = condominiumId };
                Result<List<GetPostsResponse>> result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(result);

                var response = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value
                };

                return Results.Ok(response);
            }).DisableAntiforgery();

            // post by id
            app.MapGet("/posts/{id}", async (ISender sender, Guid id, CancellationToken cancellationToken) =>
            {
                var command = new GetPostByIdCommand { Id = id };
                Result<GetPostByIdResponse> result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.NotFound(result);

                var response = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value
                };

                return Results.Ok(response);
            }).DisableAntiforgery();

            // crear un nuevo post
            app.MapPost("/posts",
               async (ISender sender, [FromForm] CreatePostCommand command, CancellationToken cancellationToken) =>
               {
                   Result<CreatePostResponse> result = await sender.Send(command, cancellationToken);
                   if (!result.IsSuccess) return Results.BadRequest(result);

                   var response = new
                   {
                       result.IsSuccess,
                       Data = result.Value
                   };
                   return Results.Ok(response);
               }
            ).DisableAntiforgery();

            // actualizar un post
            app.MapPut("/posts/{id}",
               async (ISender sender, Guid id, [FromForm] UpdatePostsCommand command, CancellationToken cancellationToken) =>
               {
                   command.Id = id;
                   Result<UpdatePostsResponse> result = await sender.Send(command, cancellationToken);
                   if (!result.IsSuccess) return Results.BadRequest(result);

                   var response = new
                   {
                       result.IsSuccess,
                       Data = result.Value
                   };
                   return Results.Ok(response);
               }
            ).DisableAntiforgery();

            // eliminar un post
            app.MapDelete("/posts/{id}",
               async (ISender sender, Guid id, CancellationToken cancellationToken) =>
               {
                   var command = new DeletePostCommand { Id = id };
                   Result<DeletePostResponse> result = await sender.Send(command, cancellationToken);

                   if (!result.IsSuccess)
                       return Results.BadRequest(result);

                   var response = new
                   {
                       result.IsSuccess,
                       Data = result.Value
                   };

                   return Results.Ok(response);
               }
            ).DisableAntiforgery();
        }
    }
}