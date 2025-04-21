using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Delete;

public class DeleteEventCommandHandler : ICommandHandler<DeleteEventCommand, Result<DeleteEventResponse>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IRepository<Event, Guid> _eventRepository;

    public DeleteEventCommandHandler(IAuthenticationProvider authenticationProvider, IRepository<Event, Guid> eventRepository)
    {
        _authenticationProvider = authenticationProvider;
        _eventRepository = eventRepository;
    }

public async Task<Result<DeleteEventResponse>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var foundEvent = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken, includes: [e => e.Condominium, e => e.CreatedBy]);

        if (foundEvent == null) return Result.Fail<DeleteEventResponse>("No se encontro el evento solicitado.");

        var doesHavePermission = _authenticationProvider.DoesHavePermission(request.RequesterId, foundEvent.CreatedById);

        if (!doesHavePermission) return Result.Fail<DeleteEventResponse>("No tienes los permisos necesarios para realizar esta accion. No creaste este evento.");

        var isUserInCondominium = await _authenticationProvider.IsUserInCondominiumAsync(request.CreatedById, foundEvent.Condominium.Id, cancellationToken);
        
        if(!isUserInCondominium) return Result.Fail<DeleteEventResponse>("No tienes los permisos necesarios para realizar esta accion. No perteneces a este condominio.");
        
        if(foundEvent.IsStarted) return Result.Fail<DeleteEventResponse>("No se puede eliminar un evento ya iniciado.");
        if(foundEvent.IsFinished) return Result.Fail<DeleteEventResponse>("No se puede eliminar un evento ya finalizado.");

        foundEvent = await _eventRepository.DeleteAsync(foundEvent.Id, cancellationToken);

        var response = foundEvent.Adapt<DeleteEventResponse>();
        
        return Result.Ok<DeleteEventResponse>(response);
    }
}