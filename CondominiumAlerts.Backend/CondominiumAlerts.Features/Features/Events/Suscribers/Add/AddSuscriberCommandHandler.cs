using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Messages;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Suscribers.Add;

public class AddSuscriberCommandHandler : ICommandHandler<AddSuscriberCommand, Result<AddSuscriberResponse>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IRepository<User, string> _userRepository;

    public AddSuscriberCommandHandler(
        IAuthenticationProvider authenticationProvider, 
        IRepository<Event, Guid> eventRepository,
        IRepository<User, string> userRepository)
    {
        _authenticationProvider = authenticationProvider;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
    }

public async Task<Result<AddSuscriberResponse>> Handle(AddSuscriberCommand request, CancellationToken cancellationToken)
    {
        var foundEvent = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken, includes: [e => e.Condominium, e => e.Suscribers]);

        if (foundEvent == null) return Result.Fail<AddSuscriberResponse>("Evento no encontrado.");
        
        var isUserInCondominium = await _authenticationProvider.IsUserInCondominiumAsync(request.UserId, foundEvent.CondominiumId, cancellationToken);
        
        if(!isUserInCondominium) return Result.Fail<AddSuscriberResponse>("Usuario no suscrito al evento: El usuario no esta en el condominio.");
        
        if (foundEvent.Suscribers.Any(u => u.Id == request.UserId))
            return Result.Fail<AddSuscriberResponse>("Usuario ya está suscrito al evento.");
        
        if(foundEvent.IsStarted) return Result.Fail<AddSuscriberResponse>("No se pueden suscribir nuevos usuarios a un evento ya iniciado.");
        
        if(foundEvent.IsFinished) return Result.Fail<AddSuscriberResponse>("No se pueden suscribir nuevos usuarios a un evento ya finalizado.");
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Fail<AddSuscriberResponse>("Usuario no encontrado.");

        // Agregar suscriptor
        foundEvent.Suscribers.Add(user);

        // Persistir el cambio
        await _eventRepository.UpdateAsync(foundEvent, cancellationToken);

        var userDto = user.Adapt<UserDto>();

        var response = new AddSuscriberResponse(
            Added: true,
            EventTitle: foundEvent.Title,
            EventId: request.EventId,
            AddedUser: userDto,
            AddedAt: DateTime.UtcNow
            );

        return Result<AddSuscriberResponse>.Ok(response);
    }
}