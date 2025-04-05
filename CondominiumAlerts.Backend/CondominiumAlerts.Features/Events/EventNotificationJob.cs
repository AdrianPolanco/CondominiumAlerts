using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Events;

public class EventNotificationJob: IInvocable
{
    
    private readonly IRepository<Event, Guid> _repository;
    private readonly IHubContext<EventHub> _hubContext;
    private readonly ILogger<EventNotificationJob> _logger;
    private readonly ScheduledEventsService _scheduledEventsService;

    public EventNotificationJob(
        IRepository<Event, Guid> repository, 
        IHubContext<EventHub> hubContext,
        ILogger<EventNotificationJob> logger,
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
        _logger.LogInformation("EventNotificationJob invoked");
        var currentDateTime = DateTime.Now;

        // Obtener los eventos cuyo inicio esté en los próximos minutos
        var eventsToStart = await _repository.GetAsync(default, e => e.Start <= currentDateTime && e.Start > currentDateTime.AddMinutes(-1));
        
        // Notificar los eventos que comienzan
        foreach (var eventItem in eventsToStart)
        {
            _logger.LogInformation($"Event {eventItem.Id} started at {eventItem.Start}");
            eventItem.IsStarted = true;
            await _hubContext.Clients.Group(eventItem.CondominiumId.ToString()).SendAsync("EventStarted", $"El evento '{eventItem.Title}' ha comenzado.");
        }

        await _repository.BulkUpdateAsync(eventsToStart, default);

        // Obtener los eventos cuyo fin esté en los próximos minutos
        var eventsToEnd = await _repository.GetAsync(default, e => e.End <= currentDateTime && e.End > currentDateTime.AddMinutes(-1));

        // Notificar los eventos que terminan
        foreach (var eventItem in eventsToEnd)
        {
            _logger.LogInformation($"Event {eventItem.Id} finished at {eventItem.End}");
            eventItem.IsFinished = true;
            await _hubContext.Clients.Group(eventItem.CondominiumId.ToString()).SendAsync("EventFinished", $"El evento '{eventItem.Title}' ha finalizado.");
        }
        
        await _repository.BulkUpdateAsync(eventsToStart, default);
    }
}