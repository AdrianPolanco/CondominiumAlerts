using Carter;
using Microsoft.AspNetCore.Mvc;
using LightResults;
using MediatR;
using CondominiumAlerts.Features.Features.Comment.Add;
using CondominiumAlerts.Features.Features.Comment.GetCommentByPost;


namespace CondominiumAlerts.Api.Endpoints
{
    public class CommentModule : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/comment", async (ISender sender, [FromQuery] Guid postId, CancellationToken cancellationToken) =>
            {
                var command = new GetCommentByPostCommand { PostId = postId };
                Result<List<GetCommentByPostResponse>> result = await sender.Send(command, cancellationToken);

                if (!result.IsSuccess)
                    return Results.BadRequest(result);

                var response = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value
                };

                return Results.Ok(response);
            }).DisableAntiforgery();

            // crear un nuevo comentario
            app.MapPost("/comment",
               async (ISender sender, [FromForm] AddCommentCommand command, CancellationToken cancellationToken) =>
               {
                   Result<AddCommentResponse> result = await sender.Send(command, cancellationToken);
                   if (!result.IsSuccess) return Results.BadRequest(result);

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
