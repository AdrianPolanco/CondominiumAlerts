using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Services;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using LightResults;
using Mapster;
using Microsoft.Extensions.Logging;
using Message = CondominiumAlerts.Domain.Aggregates.Entities.Message;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public class GetSummaryCommandHandler : ICommandHandler<GetSummaryCommand, Result<GetSummaryCommandResponse>>
{
    private readonly ILogger<GetSummaryCommandHandler> _logger;
    private readonly IRepository<Condominium, Guid> _condominiumRepository;
    private readonly IRepository<Message, Guid> _messageRepository;
    private readonly IRepository<User, string> _userRepository;
    private readonly IAIService _aiService;

    public GetSummaryCommandHandler(
        ILogger<GetSummaryCommandHandler> logger, 
        IRepository<Condominium, Guid> condominiumRepository, 
        IRepository<Message, Guid> messageRepository,
        IRepository<User, string> userRepository,
        IAIService aiService)
    {
        _logger = logger;
        _condominiumRepository = condominiumRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _aiService = aiService;
    }
    
    public async Task<Result<GetSummaryCommandResponse>> Handle(GetSummaryCommand request, CancellationToken cancellationToken)
    {
        //Validacion de condominio
        var condominium = await _condominiumRepository.GetByIdAsync(request.CondominiumId, cancellationToken);
        if (condominium is null)
        {
            var errorMessage = $"Condominio no encontrado. CondominiumId: {request.CondominiumId}";
            _logger.LogWarning(errorMessage);
            return Result.Fail<GetSummaryCommandResponse>(errorMessage);
        }
        
        // Validación de usuario
        var user = await _userRepository.GetByIdAsync(request.TriggeredBy, cancellationToken);
        if (user is null)
        {
            var errorMessage = $"Usuario no encontrado. UserId: {request.TriggeredBy}";
            _logger.LogWarning(errorMessage);
            return Result.Fail<GetSummaryCommandResponse>(errorMessage);
        }
        // Validación de mensajes
        var messages = await _messageRepository.GetAsync(filter: m => m.CondominiumId == request.CondominiumId && m.CreatedAt > DateTime.UtcNow.AddHours(-24), cancellationToken: cancellationToken);
        if (messages.Count == 0)
        {
            var errorMessage = $"No se encontraron mensajes en el condominio. CondominiumId: {request.CondominiumId}, Mensajes: 0";
            _logger.LogWarning(errorMessage);
            return Result.Fail<GetSummaryCommandResponse>(errorMessage);
        }
        
        var recoveredMessagesLog = $"Mensajes recuperados exitosamente. CondominiumId: {request.CondominiumId}. Cantidad de mensajes: {messages.Count}";
        _logger.LogInformation(recoveredMessagesLog);
        
        var messagesDto = messages.Adapt<List<MessageDto>>();
        
        _logger.LogInformation("MessagesDTO: {dto}", messagesDto);

        var summary = await _aiService.GenerateSummary(messagesDto, user, condominium, cancellationToken);
        
        if(summary is null) return Result.Fail<GetSummaryCommandResponse>("Hubo un error al resumir la conversacion.");
        
        var response = new GetSummaryCommandResponse(summary);
        
        var successMessage = $"Resumen solicitado exitosamente. CondominiumId: {request.CondominiumId}, UserId: {request.TriggeredBy}, Mensajes: {messages.Count}";
        _logger.LogInformation(successMessage);
        
        return Result.Ok<GetSummaryCommandResponse>(response);
    }
}