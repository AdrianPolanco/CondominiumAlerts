using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Features.Events.Create;

public class CreateEventCommandHandler: ICommandHandler<CreateEventCommand, Result<CreateEventResponse>>
{
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IRepository<Condominium, Guid> _condominiumRepository;
    private readonly IRepository<User, string> _userRepository;

    public CreateEventCommandHandler(
        IRepository<Event, Guid> eventRepository, 
        IRepository<Condominium, Guid> condominiumRepository, 
        IRepository<User, string> userRepository)
    {
        _eventRepository = eventRepository;
        _condominiumRepository = condominiumRepository;
        _userRepository = userRepository;
    }
    
    
    public async Task<Result<CreateEventResponse>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var createdBy = await _userRepository.GetByIdAsync(request.CreatedById, cancellationToken);
        var condominium = await _condominiumRepository.GetByIdAsync(request.CondominiumId, cancellationToken);

        if (createdBy is null || condominium is null)
            return Result.Fail<CreateEventResponse>("Usuario o condominio no encontrado.");

        var (start, end) = TimeHelper.ConvertToUtc(request.Start, request.End);
        
        if(start <= DateTime.UtcNow) return Result.Fail<CreateEventResponse>("No se puede crear un evento con una fecha de inicio anterior o igual al tiempo actual.");
        if(end <= DateTime.UtcNow) return Result.Fail<CreateEventResponse>("No se puede crear un evento con una fecha de finalizacion anterior o igual al tiempo actual.");
        if(end == start) return Result.Fail<CreateEventResponse>("No se puede crear un evento con una fecha de inicio y finalizacion iguales.");
        
        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Start = start,
            End = end,
            CreatedById = createdBy.Id,
            CreatedBy = createdBy,
            CondominiumId = condominium.Id,
            Condominium = condominium,
            IsToday = start.Date == DateTime.UtcNow.Date,
            Suscribers = new(){createdBy}
        };

        await _eventRepository.CreateAsync(newEvent, cancellationToken);
        
        var createEventResponse = newEvent.Adapt<CreateEventResponse>();

        return Result.Ok(createEventResponse);
    }
}