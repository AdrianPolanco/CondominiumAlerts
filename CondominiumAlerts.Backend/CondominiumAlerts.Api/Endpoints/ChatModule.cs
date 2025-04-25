using System.Net;
using System.Security.Claims;
using Carter;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CondominiumAlerts.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace CondominiumAlerts.Api.Endpoints
{
    public class ChatModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapHub<ChatHub>("/hubs/chat");

            app.MapPost("/chat/image", async (
                [FromServices] IAsyncPolicy retryPolicy,
                [FromServices] Cloudinary cloudinary,
                ClaimsPrincipal claims,
                [FromForm]IFormFile file) =>
            {
                var currentUserId = claims.FindFirst("user_id")?.Value;

                if (string.IsNullOrWhiteSpace(currentUserId.Trim())) return Results.Unauthorized();
                    
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                };

                var uploadResult =
                    await retryPolicy.ExecuteAsync(async () => await cloudinary.UploadAsync(uploadParams));

                var isSuccess = uploadResult.StatusCode == HttpStatusCode.OK;

                var response = new
                {
                    isSuccess = isSuccess,
                    Data = new
                    {
                        ImageUrl = isSuccess ? uploadResult.SecureUrl.ToString() : null
                    }
                };

                if (!isSuccess) return Results.InternalServerError(response);

                return Results.Ok(response);
            })
                .DisableAntiforgery()
                .RequireAuthorization();
        }
    }
}
