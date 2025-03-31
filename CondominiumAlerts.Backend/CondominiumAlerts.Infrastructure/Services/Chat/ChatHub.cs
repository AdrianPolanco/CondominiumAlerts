using Microsoft.AspNetCore.SignalR;

namespace CondominiumAlerts.Infrastructure.Services.Chat
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string condominiumId, string text, string senderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, condominiumId);
        }

        public async Task JoinGroup(string condominiumId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, condominiumId);      
        }

        public async Task LeaveGroup(string condominiumId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, condominiumId);
        }
    }
}
