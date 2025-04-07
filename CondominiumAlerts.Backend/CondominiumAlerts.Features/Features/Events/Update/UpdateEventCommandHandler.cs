using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Update;

public class UpdateEventCommandHandler: ICommandHandler<UpdateEventCommand, Result<UpdateEventResponse>>
{
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IAuthenticationProvider _authenticationProvider;

    public UpdateEventCommandHandler(IRepository<Event, Guid> eventRepository, IAuthenticationProvider authenticationProvider)
    {
        _eventRepository = eventRepository;
        _authenticationProvider = authenticationProvider;
    }
    
    public async Task<Result<UpdateEventResponse>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var areCreatorIdsEmpty = string.IsNullOrEmpty(request.CreatedById) || string.IsNullOrWhiteSpace(request.EditorId); 
        if(request.CreatedById != request.EditorId || areCreatorIdsEmpty) 
            return Result<UpdateEventResponse>.Fail("No tienes los permisos requeridos para crear esta accion.");

        var isUserInCondominium = await _authenticationProvider.IsUserInCondominiumAsync(request.EditorId, request.CondominiumId, cancellationToken);

        if (!isUserInCondominium)
            return Result<UpdateEventResponse>.Fail(
                "No tienes los permisos requeridos para crear esta accion. Debes pertenecer al condominio.");
        
        var events = await _eventRepository.GetAsync(
            cancellationToken, 
            e => e.Id == request.Id 
                 && e.CreatedById == request.CreatedById 
                 && e.CondominiumId == request.CondominiumId);
        
        if(!events.Any()) return Result<UpdateEventResponse>.Fail("No se ha encontrado el evento.");
        
        var foundEvent  = events.FirstOrDefault();

        if (foundEvent == null) return Result<UpdateEventResponse>.Fail("No se encontro el evento solicitado.");

        if (foundEvent.IsStarted) return Result<UpdateEventResponse>.Fail("No se puede editar un evento ya iniciado.");
        if (foundEvent.IsFinished) return Result<UpdateEventResponse>.Fail("No se puede editar un evento ya finalizado.");
        
        var (start, end) = TimeHelper.ConvertToUtc(request.Start, request.End);
        var doesHavePermissions = _authenticationProvider.DoesHavePermission(request.EditorId, foundEvent.CreatedById);
        
        if(!doesHavePermissions) return Result<UpdateEventResponse>.Fail("No tienes los permisos necesarios para realizar esta accion. No creaste este evento.");
        
        if(start <= DateTime.UtcNow) return Result.Fail<UpdateEventResponse>("No se puede crear un evento con una fecha de inicio anterior o igual al tiempo actual.");
        if (end <= DateTime.UtcNow) 
            return Result<UpdateEventResponse>.Fail("No se puede editar una fecha de finalizacion del evento anterior o igual al tiempo actual.");
        if (end == start) 
            return Result<UpdateEventResponse>.Fail("No se puede editar unas fechas de inicio y finalizacion iguales.");
        if(end < start) return Result.Fail<UpdateEventResponse>("No se puede crear un evento con una fecha de inicio posterior a la fecha de finalizacion.");
        if(start == foundEvent.Start && end == foundEvent.End
           && foundEvent.Title == request.Title
           && foundEvent.Description == request.Description)  return Result<UpdateEventResponse>.Fail("Se aborto la actualizacion ya que no hay cambios en los campos correspondientes.");

        foundEvent.Title = request.Title;
        foundEvent.Description = request.Description;
        foundEvent.Start = start;
        foundEvent.End = end;
        foundEvent.IsToday = start.Date == DateTime.UtcNow.Date;
        foundEvent.IsFinished = end <= DateTime.UtcNow.Date;
        
        foundEvent = await _eventRepository.UpdateAsync(foundEvent, cancellationToken);

        var response = foundEvent.Adapt<UpdateEventResponse>();

        return Result<UpdateEventResponse>.Ok(response);
    }
}