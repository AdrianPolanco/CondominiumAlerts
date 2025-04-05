using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Events;

public class EventHub: Hub
{
    private readonly ILogger<EventHub> _logger;

    public EventHub(ILogger<EventHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinGroup(string eventId, string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
        _logger.LogInformation($"User {userId} joined group {Context.ConnectionId}");
    }

    public async Task LeaveGroup(string eventId, string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId);
        _logger.LogInformation($"User {userId} left group {Context.ConnectionId}");
    }
}