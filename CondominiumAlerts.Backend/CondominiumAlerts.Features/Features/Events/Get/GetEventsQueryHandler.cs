using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Get;

public class GetEventsQueryHandler: IQueryHandler<GetEventsQuery, Result<List<GetEventsQueryResponse>>>
{
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;

    public GetEventsQueryHandler(
        IRepository<Event, Guid> eventRepository,
        IRepository<CondominiumUser, Guid> condominiumUserRepository)
    {
        _eventRepository = eventRepository;
        _condominiumUserRepository = condominiumUserRepository;
    }
    
    public async Task<Result<List<GetEventsQueryResponse>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        if (request.CondominiumId == Guid.Empty
            || string.IsNullOrEmpty(request.UserId) 
            || request.RequesterId != request.UserId)
            return Result<List<GetEventsQueryResponse>>.Fail("No tienes permisos para ejecutar esta accion.");

        var condominiumUsers = await _condominiumUserRepository.GetAsync(
            cancellationToken,
            cu => cu.CondominiumId == request.CondominiumId && cu.UserId == request.UserId
        );
        
        if(!condominiumUsers.Any()) return Result.Fail<List<GetEventsQueryResponse>>("El usuario no se encuentra en el condominio dado.");

        var events = await _eventRepository.GetAsync(
            cancellationToken,
            e => e.CondominiumId == request.CondominiumId,
            includes: [e => e.CreatedBy, e => e.Suscribers]);

        events = events.OrderByDescending(e => e.CreatedAt).ToList();
        
        var response = events.Adapt<List<GetEventsQueryResponse>>();
        
        return Result<List<GetEventsQueryResponse>>.Ok(response);
    }
}