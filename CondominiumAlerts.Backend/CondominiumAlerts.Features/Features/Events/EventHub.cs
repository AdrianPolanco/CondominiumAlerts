using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Events;

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
        _logger.LogInformation($"User {userId} joined group {Context.ConnectionId} subscribed for event {eventId}");
    }

    public async Task LeaveGroup(string eventId, string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId);
        _logger.LogInformation($"User {userId} left group {Context.ConnectionId} subscribed for event {eventId}");
    }
    
    public async Task<string> Echo(string message)
    {
        _logger.LogInformation($"Echo received: {message}");
        return $"Server received: {message} at {DateTime.Now}";
    }
}