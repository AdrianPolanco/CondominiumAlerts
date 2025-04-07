using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;

namespace CondominiumAlerts.Features.Features.Events.Update;

public class UpdateEventCommandHandler: ICommandHandler<UpdateEventCommand, Result<UpdateEventResponse>>
{
    private readonly IRepository<Event, Guid> _eventRepository;

    public UpdateEventCommandHandler(IRepository<Event, Guid> eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<Result<UpdateEventResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var areCreatorIdsEmpty = string.IsNullOrEmpty(request.CreatedById) || string.IsNullOrWhiteSpace(request.EditorId); 
        if(request.CreatedById != request.EditorId || areCreatorIdsEmpty) 
            return Result<UpdateEventResponse>.Fail("No tienes los permisos requeridos para crear esta accion.");
        
        return Result<UpdateEventResponse>.Fail("No tienes los permisos requeridos para crear esta accion.");
    }
}