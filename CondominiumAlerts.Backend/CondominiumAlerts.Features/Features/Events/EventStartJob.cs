using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Events;

public class EventStartJob : IInvocable, IInvocableWithPayload<Guid>
{
    private readonly IRepository<Event, Guid> _repository;
    private readonly IHubContext<EventHub> _hubContext;
    private readonly ILogger<EventStartJob> _logger;
    private readonly ScheduledEventsService _scheduledEventsService;

    public EventStartJob(
        IRepository<Event, Guid> repository, 
        IHubContext<EventHub> hubContext,
        ILogger<EventStartJob> logger,
        ScheduledEventsService scheduledEventsService
        )
    {
        _repository = repository;
        _hubContext = hubContext;
        _logger = logger;
        _scheduledEventsService = scheduledEventsService;
    }

    public async Task Invoke()
    {
        
        var (eventId, scheduledTime) = _scheduledEventsService.GetNextEventToStart();
        if(eventId == Guid.Empty || scheduledTime == null) return;
        var ev = await _repository.GetByIdAsync(eventId, default);
        if (ev is null || ev.IsStarted || ev.IsFinished) return;

        ev.IsStarted = true;
        await _repository.UpdateAsync(ev);

        await _hubContext.Clients.Group(ev.Id.ToString())
            .SendAsync("EventStarted", new { ev.Id, ev.Title });

        _logger.LogInformation($"Evento iniciado: {ev.Title}");
    }

    public Guid Payload { get; set; }
}
