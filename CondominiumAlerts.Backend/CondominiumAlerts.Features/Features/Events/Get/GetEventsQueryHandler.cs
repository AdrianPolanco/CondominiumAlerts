using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Get;

public class GetEventsQueryHandler: IQueryHandler<GetEventsQuery, Result<List<GetEventsQueryResponse>>>
{
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IAuthenticationProvider _authenticationProvider;

    public GetEventsQueryHandler(
        IRepository<Event, Guid> eventRepository,
        IAuthenticationProvider authenticationProvider)
    {
        _eventRepository = eventRepository;
        _authenticationProvider = authenticationProvider;
    }
    
    public async Task<Result<List<GetEventsQueryResponse>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        if (request.CondominiumId == Guid.Empty
            || string.IsNullOrEmpty(request.UserId) 
            || request.RequesterId != request.UserId)
            return Result<List<GetEventsQueryResponse>>.Fail("No tienes permisos para ejecutar esta accion.");

        var isUserInCondominium = await _authenticationProvider.IsUserInCondominiumAsync(request.RequesterId, request.CondominiumId, cancellationToken);
        
        if(!isUserInCondominium) return Result.Fail<List<GetEventsQueryResponse>>("El usuario no se encuentra en el condominio dado.");

        var events = await _eventRepository.GetAsync(
            cancellationToken,
            e => e.CondominiumId == request.CondominiumId,
            includes: [e => e.CreatedBy, e => e.Suscribers]);

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
        
        return Result<List<GetEventsQueryResponse>>.Ok(responses);
    }
}