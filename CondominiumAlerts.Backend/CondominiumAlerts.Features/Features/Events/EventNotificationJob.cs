using System.Security.Cryptography;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using Coravel.Invocable;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Events;

public class EventNotificationJob : IInvocable
{

    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IHubContext<EventHub> _hubContext;
    private readonly ILogger<EventNotificationJob> _logger;
    private readonly ScheduledEventsService _scheduledEventsService;

    public EventNotificationJob(
        IRepository<Event, Guid> eventRepository,
        IRepository<Notification, Guid> notificationRepository,
        IHubContext<EventHub> hubContext,
        ILogger<EventNotificationJob> logger,
        ScheduledEventsService scheduledEventsService
    )
    {
        _eventRepository = eventRepository;
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _logger = logger;
        _scheduledEventsService = scheduledEventsService;
    }
    public async Task Invoke()
    {
        _logger.LogInformation("EventNotificationJob invoked");
        var currentDateTime = DateTime.UtcNow;

        // Obtener los eventos cuyo inicio esté en los próximos minutos
        var eventsToStart = await _eventRepository.GetAsync(
            default,
            e => e.Start <= currentDateTime && e.Start > currentDateTime.AddMinutes(-1) && !e.IsStarted,
            includes: [e => e.Condominium, e => e.Suscribers]
            );

        var startedEventsNotifications = new List<Notification>();
        var endedEventsNotifications = new List<Notification>();

        // Notificar los eventos que comienzan
        foreach (var eventItem in eventsToStart)
        {
            try
            {
                _logger.LogInformation($"Event {eventItem.Id} started at {eventItem.Start}");
                eventItem.IsStarted = true;
                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    Title = $"El evento '{eventItem.Title}' ha comenzado en el condominio {eventItem.Condominium.Name}.",
                    Description = eventItem.Description,
                    CondominiumId = eventItem.CondominiumId,
                    EventId = eventItem.Id,
                };

                startedEventsNotifications.Add(notification);

                await _hubContext.Clients.Group(eventItem.Id.ToString()).SendAsync("EventStarted", notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al enviar la notificación de evento iniciado");
            }

        }

        // Obtener los eventos cuyo fin esté en los próximos minutos
        var eventsToEnd = await _eventRepository.GetAsync(
            default,
            e => e.End <= currentDateTime && e.End > currentDateTime.AddMinutes(-1) && !e.IsFinished,
            includes: [e => e.Condominium]
            );

        // Notificar los eventos que terminan
        foreach (var eventItem in eventsToEnd)
        {
            try
            {
                _logger.LogInformation($"Event {eventItem.Id} finished at {eventItem.End}");
                eventItem.IsFinished = true;

                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    Title = $"El evento '{eventItem.Title}' ha finalizado en el condominio {eventItem.Condominium.Name}.",
                    Description = eventItem.Description,
                    CondominiumId = eventItem.CondominiumId,
                    EventId = eventItem.Id
                };

                endedEventsNotifications.Add(notification);
                await _hubContext.Clients.Group(eventItem.Id.ToString()).SendAsync("EventFinished", notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al enviar la notificación de evento finalizado");
            }
        }

        try
        {
            var notificationsToCreate = startedEventsNotifications.Concat(endedEventsNotifications).ToList();
            await Task.Delay(RandomNumberGenerator.GetInt32(100, 10000));

            var notisCreated = (await _notificationRepository.GetAsync(
                default,
                filter: n => notificationsToCreate
                             .Select(x => x.CondominiumId)
                             .Contains(n.CondominiumId)
                             && notificationsToCreate
                             .Select(x => x.EventId)
                             .Contains(n.EventId)
                             && notificationsToCreate
                             .Select(x => x.Description)
                             .Contains(n.Description)
                )
            ).Select(x => new
            {
                x.CondominiumId,
                x.EventId,
                x.Description
            }).ToList();

            await _notificationRepository.BulkInsertAsync(
                notificationsToCreate.Where(x1 =>
                !notisCreated.Any(x2 =>
                    x2.CondominiumId == x1.CondominiumId &&
                    x2.EventId == x1.EventId &&
                    x2.Description == x1.Description
                )).ToList(), default
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al guardar notificaciones de eventos.");
        }


        try
        {
            // Unimos ambas listas y hacemos un solo BulkUpdate
            var eventsToUpdate = eventsToStart.Concat(eventsToEnd).ToList();
            await _eventRepository.BulkUpdateAsync(eventsToUpdate, default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar el status de los eventos.");
        }

    }
}