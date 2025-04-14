
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Notifications.Get
{
    public class GetNotificationsOfUserQueryHandler : IQueryHandler<GetNotificationsOfUserQuery, Result<List<NotificationDto>>>
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly ILogger<GetNotificationsOfUserQueryHandler> _logger;

        public GetNotificationsOfUserQueryHandler(
            IRepository<Notification, Guid> notificationRepository,
            ILogger<GetNotificationsOfUserQueryHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }
        
        public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsOfUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var notifications = await _notificationRepository.GetAsync(
                    cancellationToken: cancellationToken,
                    filter: n => n.ReceiverUserId == request.UserId || n.ReceiverUserId == null,
                    includes: [n => n.LevelOfPriority],
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