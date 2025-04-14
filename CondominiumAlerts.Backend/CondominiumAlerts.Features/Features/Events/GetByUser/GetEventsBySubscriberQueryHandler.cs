using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Events.Get;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.GetByUser;

public class GetEventsBySubscriberQueryHandler: IQueryHandler<GetEventsBySubscriberQuery, Result<GetEventsBySubscriberQueryResponse>>
{
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IAuthenticationProvider _authenticationProvider;

    public GetEventsBySubscriberQueryHandler(
        IRepository<Event, Guid> eventRepository,
        IAuthenticationProvider authenticationProvider)
    {
        _eventRepository = eventRepository;
        _authenticationProvider = authenticationProvider;
    }

    public async Task<Result<GetEventsBySubscriberQueryResponse>> Handle(GetEventsBySubscriberQuery request, CancellationToken cancellationToken)
    {
        
        var events = await _eventRepository.GetAsync(
            cancellationToken,
            e => e.Suscribers.Any(u => u.Id == request.UserId),
            includes: [e => e.Suscribers]);

        events = events.OrderByDescending(e => e.CreatedAt).ToList();
        
        var responses = events.Select(e =>
        {
            var dto = e.Adapt<GetEventsQueryResponse>();
            dto = dto with
            {
                IsSuscribed = e.Suscribers.Any(s => s.Id == request.UserId)
            };
            return dto;
        }).ToList();
        
        var response = new GetEventsBySubscriberQueryResponse(responses);   
        
        return Result<GetEventsBySubscriberQueryResponse>.Ok(response);
    }
}