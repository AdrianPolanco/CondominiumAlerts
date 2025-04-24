using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Notifications.Get;

public class GetEventRelatedNotificationsQueryHandler: IQueryHandler<GetEventRelatedNotificationsQuery, Result<GetEventRelatedNotificationsQueryResponse>>
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IRepository<Event, Guid> _eventRepository;

    public GetEventRelatedNotificationsQueryHandler(IRepository<Notification, Guid> notificationRepository, IRepository<Event, Guid> eventRepository)
    {
        _notificationRepository = notificationRepository;
        _eventRepository = eventRepository;
    }
    
    public async Task<Result<GetEventRelatedNotificationsQueryResponse>> Handle(GetEventRelatedNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (request.RequesterId != request.UserId) 
            return Result.Fail<GetEventRelatedNotificationsQueryResponse>("No tienes permisos para acceder a las notificaciones de este usuario.");

        var eventsSubscribedByUser = await _eventRepository.GetAsync(
            cancellationToken,
            filter: e => e.Suscribers.Any(u => u.Id == request.RequesterId),
            includes: [e => e.Suscribers]
        );

        var eventIds = eventsSubscribedByUser
            .Select(e => e.Id)
            .ToHashSet(); // HashSet mejora rendimiento en búsquedas

        var notifications = await _notificationRepository.GetAsync(
            cancellationToken,
            filter: n => n.EventId.HasValue &&
                         eventIds.Contains(n.EventId.Value) ||
                         n.ReceiverUserId == request.UserId,
            includes: [n => n.LevelOfPriority, n => n.NotificationUsers]
        );

        notifications = notifications.OrderByDescending(n => n.CreatedAt).ToList();

        List<NotificationDto> notiRes = notifications.ConvertAll(x => new NotificationDto()
        {
            Id = x.Id,
            Title = x.Title,
            CondominiumId = x.CondominiumId,
            CreatedAt = x.CreatedAt,
            Description = x.Description,
            Read = x.NotificationUsers
                        ?.FirstOrDefault(n => n.UserId == request.UserId)
                        ?.Read ?? false,
            LevelOfPriority = x.LevelOfPriority is null ? null
                    : new LevelOfPriorityDto()
                    {
                        Id = x.LevelOfPriority.Id,
                        Priority = x.LevelOfPriority.Priority,
                        Title = x.Title
                    }
        });

        var response = new GetEventRelatedNotificationsQueryResponse(
            notiRes
        );

        return Result.Ok<GetEventRelatedNotificationsQueryResponse>(response);
    }
}