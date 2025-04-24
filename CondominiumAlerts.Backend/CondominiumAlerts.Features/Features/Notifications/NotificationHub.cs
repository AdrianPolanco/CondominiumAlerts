using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Notifications
{
    public class NotificationHub : Hub
    {
        public const string ReciveNotification = "ReceiveNotification";

        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        // Join a notification group (typically by condominium ID)
        public async Task JoinGroup(string condominiumId, string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, condominiumId);
            _logger.LogInformation($"User {userId} joined notification group for condominium {condominiumId}");
        }

        // Leave a notification group
        public async Task LeaveGroup(string condominiumId, string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, condominiumId);
            _logger.LogInformation($"User {userId} left notification group for condominium {condominiumId}");
        }
    }
}