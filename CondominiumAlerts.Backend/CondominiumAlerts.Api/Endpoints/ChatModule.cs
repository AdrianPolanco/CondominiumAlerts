using Carter;
using CondominiumAlerts.Api.Hubs;

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
