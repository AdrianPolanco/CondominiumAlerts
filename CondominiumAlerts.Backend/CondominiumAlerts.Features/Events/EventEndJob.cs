using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Events;

public class EventEndJob : IInvocable
{
    private readonly IHubContext<EventHub> _hubContext;
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly ILogger<EventEndJob> _logger;
    private readonly Guid _eventId;
    private readonly ScheduledEventsService _scheduledEventsService;

    public EventEndJob(
        IHubContext<EventHub>  hubContext, 
        IRepository<Event, Guid> eventRepository, 
        ILogger<EventEndJob> logger,
        Guid eventId,
        ScheduledEventsService scheduledEventsService)
    {
        _hubContext = hubContext;
        _eventRepository = eventRepository;
        _logger = logger;
        _eventId = eventId;
        _scheduledEventsService = scheduledEventsService;
    }

    public async Task Invoke()
    {
        var (eventId, scheduledTime) = _scheduledEventsService.GetNextEventToStart();
        if(eventId == Guid.Empty || scheduledTime == null) return;
        
        var ev = await _eventRepository.GetByIdAsync(_eventId, default);

        if (ev != null && !ev.IsFinished)
        {
            ev.IsFinished = true;
            await _eventRepository.UpdateAsync(ev, default);

            await _hubContext.Clients.Group(ev.Id.ToString())
                .SendAsync("EventStarted", new { ev.Id, ev.Title });
            _logger.LogInformation($"Evento terminado: {ev.Title}");
        }
    }
}
