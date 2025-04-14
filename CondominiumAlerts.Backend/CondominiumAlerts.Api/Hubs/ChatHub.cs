using CondominiumAlerts.Features.Features.Messages.Create;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CondominiumAlerts.Api.Hubs
{
    public class ChatHub(ISender sender) : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(Guid condominiumId, string text, string creatorUserId, string? receiverUserId)
        {
            AddMessageCommand command = new() { CondominiumId = condominiumId, Text = text, CreatorUserId = creatorUserId, ReceiverUserId = receiverUserId};
            var result = await sender.Send(command);
            await Clients.Group(condominiumId.ToString()).SendAsync("NewMessage", result.Value);
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
