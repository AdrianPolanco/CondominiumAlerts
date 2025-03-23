using Carter;
using CondominiumAlerts.Features.Features.Posts.Create;
using CondominiumAlerts.Features.Features.Posts.Get;
using LightResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.Api.Endpoints
{
    public class PostsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
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
           // TODO: Add anti forgery token in frontend: https://stackoverflow.com/a/77191406
           ).DisableAntiforgery();
        }
    }
}