
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public class GetNotificationsOfUserQueryHandler
    : IQueryHandler<GetNotificationsOfUserQuery, Result<List<NotificationDto>>>
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
        private readonly ILogger<GetNotificationsOfUserQueryHandler> _logger;
        private readonly IRepository<Event, Guid> _eventRepository;

        public GetNotificationsOfUserQueryHandler(
            IRepository<Notification, Guid> notificationRepository,
            ILogger<GetNotificationsOfUserQueryHandler> logger,
            IRepository<CondominiumUser, Guid> condominiumUserRepository,
            IRepository<Event, Guid> eventRepository)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
            _condominiumUserRepository = condominiumUserRepository;
            _eventRepository = eventRepository;
        }

        public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsOfUserQuery request, CancellationToken cancellationToken)
        {

            if (request.RequesterId != request.UserId)
            {
                return Result.Fail<List<NotificationDto>>("No tienes permisos para acceder a las notificaciones de este usuario.");
            }

            try
            {
                IEnumerable<Guid> condominiumUsers = (await _condominiumUserRepository.GetAsync(
                    cancellationToken,
                    filter: cu => cu.UserId == request.UserId
                )).Select(x => x.CondominiumId);

                var eventIds = (await _eventRepository.GetAsync(
                    cancellationToken,
                    filter: e => e.Suscribers.Any(u => u.Id == request.RequesterId),
                    includes: [e => e.Suscribers]
                )).Select(e => e.Id);

                var notifications = await _notificationRepository.GetAsync(
                    cancellationToken: cancellationToken,
                    filter: n => n.ReceiverUserId == request.UserId
                                 || n.ReceiverUserId == null
                                 || (n.CondominiumId.HasValue
                                     && condominiumUsers.Contains(n.CondominiumId.Value))
                                 || (n.EventId.HasValue &&
                                     eventIds.Contains(n.EventId.Value)),

                    includes: [n => n.LevelOfPriority!],
                    readOnly: true
                );

                if (!notifications.Any())
                {
                    _logger.LogInformation($"No notifications found for user {request.UserId} ");
                    return Result.Ok(new List<NotificationDto>());
                }

                var result = notifications.Adapt<List<NotificationDto>>();
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving notifications for user {request.UserId}");
                return Result.Fail<List<NotificationDto>>("An error occurred while retrieving notifications");
            }
        }
    }
}