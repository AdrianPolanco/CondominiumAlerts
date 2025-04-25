using Carter;
using Microsoft.AspNetCore.Mvc;
using LightResults;
using MediatR;
using CondominiumAlerts.Features.Features.Comment.Add;
using CondominiumAlerts.Features.Features.Comment.GetCommentByPost;
using CondominiumAlerts.Features.Features.Comment.Update;


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

            //Actualizar comentario
            app.MapPut("/comment/{id}",
               async (ISender sender, Guid id, [FromForm] UpdateCommentCommand command, CancellationToken cancellationToken) =>
               {
                   command.Id = id;
                   Result<UpdateCommentResponse> result = await sender.Send(command, cancellationToken);
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
