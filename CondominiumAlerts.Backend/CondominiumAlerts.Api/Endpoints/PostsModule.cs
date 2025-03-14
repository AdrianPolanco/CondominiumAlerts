using Carter;
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
            app.MapGet("/posts", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new GetPostsCommand();
                Result<List<GetPostsResponse>> result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(result);

                var response = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value
                };

                return Results.Ok(response);
            });
        }
    }
}