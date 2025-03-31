using Carter;
using CondominiumAlerts.Infrastructure.Services.Chat;

namespace CondominiumAlerts.Api.Endpoints
{
    public class ChatModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapHub<ChatHub>("/hubs/chat");
        }
    }
}
