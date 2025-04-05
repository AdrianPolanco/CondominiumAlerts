using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using Coravel.Scheduling.Schedule.Interfaces;
using LightResults;
using Mapster;

namespace CondominiumAlerts.Features.Events.Create;

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

        var startDateTime = DateTime.Parse(request.Start.ToString()).ToUniversalTime();
        var endDateTime = DateTime.Parse(request.End.ToString()).ToUniversalTime();

        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Start = startDateTime,
            End = endDateTime,
            CreatedById = createdBy.Id,
            CreatedBy = createdBy,
            CondominiumId = condominium.Id,
            Condominium = condominium
        };

        await _eventRepository.CreateAsync(newEvent, cancellationToken);
        
        var createEventResponse = newEvent.Adapt<CreateEventResponse>();

        return Result.Ok(createEventResponse);
    }
}