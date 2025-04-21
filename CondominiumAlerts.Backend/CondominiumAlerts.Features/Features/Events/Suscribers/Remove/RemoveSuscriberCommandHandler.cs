using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Messages;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Suscribers.Remove;

public class RemoveSuscriberCommandHandler: ICommandHandler<RemoveSuscriberCommand, Result<RemoveSuscriberResponse>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IRepository<User, string> _userRepository;

    public RemoveSuscriberCommandHandler(
        IAuthenticationProvider authenticationProvider, 
        IRepository<Event, Guid> eventRepository,
        IRepository<User, string> userRepository)
    {
        _authenticationProvider = authenticationProvider;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
    }
    public async Task<Result<RemoveSuscriberResponse>> Handle(RemoveSuscriberCommand request, CancellationToken cancellationToken)
    {
        var foundEvent = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken, includes: [e => e.Condominium, e => e.Suscribers]);

        if (foundEvent == null) return Result.Fail<RemoveSuscriberResponse>("Evento no encontrado.");
        
        if(foundEvent.IsStarted) return Result.Fail<RemoveSuscriberResponse>("No se pueden desuscribir nuevos usuarios a un evento ya iniciado.");
        
        if(foundEvent.IsFinished) return Result.Fail<RemoveSuscriberResponse>("No se pueden desuscribir nuevos usuarios a un evento ya finalizado.");

        var user = foundEvent.Suscribers.FirstOrDefault(s => s.Id == request.UserId);
        
        if (user is null)
            return Result.Fail<RemoveSuscriberResponse>("Usuario no suscrito al evento.");
        
        if(user.Id == foundEvent.CreatedById) return Result.Fail<RemoveSuscriberResponse>("No se puede desuscribir al usuario creador del evento. Si desea cancelar el evento, intente eliminarlo.");
        foundEvent.Suscribers.Remove(user);
        
        await _eventRepository.UpdateAsync(foundEvent, cancellationToken);

        var userDto = user.Adapt<UserDto>();

        var response = new RemoveSuscriberResponse(
            Removed: true,
            EventTitle: foundEvent.Title,
            EventId: request.EventId,
            RemovedUser: userDto,
            RemovedAt: DateTime.UtcNow
        );

        return Result<RemoveSuscriberResponse>.Ok(response);  
    }
}