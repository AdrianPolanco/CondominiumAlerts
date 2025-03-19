using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Services;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using LightResults;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Message = CondominiumAlerts.Domain.Aggregates.Entities.Message;


namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public class GetSummaryCommandHandler : ICommandHandler<GetSummaryCommand, Result<GetSummaryCommandResponse>>
{
    private readonly IHubContext<SummaryHub> _hubContext;
    private readonly ILogger<GetSummaryCommandHandler> _logger;
    private readonly IRepository<Message, Guid> _messageRepository;
    private readonly IAiService _aiService;

    public GetSummaryCommandHandler(
        ILogger<GetSummaryCommandHandler> logger, 
        IRepository<Message, Guid> messageRepository,
        IAiService aiService,
        IHubContext<SummaryHub> hubContext
        )
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _aiService = aiService;
        _hubContext = hubContext;
    }
    
    public async Task<Result<GetSummaryCommandResponse>> Handle(GetSummaryCommand request, CancellationToken cancellationToken)
    {
        // await _hubContext.Clients.Group(request.CondominiumId.ToString()).SendAsync("NotifyProcessingStarted", $"Se inició el procesamiento para el condominio {condominium.Name}.", cancellationToken);
        
        // Validación de mensajes
        var messages = await _messageRepository.GetAsync(filter: m => m.CondominiumId == request.Condominium.Id && m.CreatedAt > DateTime.UtcNow.AddHours(-24), cancellationToken: cancellationToken);
        if (messages.Count == 0)
        {
            var errorMessage = $"No se encontraron mensajes en el condominio. CondominiumId: {request.Condominium.Id}, Mensajes: 0";
            _logger.LogWarning(errorMessage);
            return Result.Fail<GetSummaryCommandResponse>(errorMessage);
        }
        
        var recoveredMessagesLog = $"Mensajes recuperados exitosamente. CondominiumId: {request.Condominium.Id}. Cantidad de mensajes: {messages.Count}";
        _logger.LogInformation(recoveredMessagesLog);
        
        var messagesDto = messages.Adapt<List<MessageSummaryDto>>();
        
        _logger.LogInformation("MessagesDTO: {dto}", messagesDto);

        var summary = await _aiService.GenerateSummary(messagesDto, request.TriggeredByUser, request.Condominium, cancellationToken);
        
        if(summary is null) return Result.Fail<GetSummaryCommandResponse>("Hubo un error al resumir la conversacion.");
        
        var response = new GetSummaryCommandResponse(summary);
        
        var successMessage = $"Resumen solicitado exitosamente. CondominiumId: {request.Condominium.Id}, UserId: {request.TriggeredByUser}, Mensajes: {messages.Count}";
        _logger.LogInformation(successMessage);
        
        return Result.Ok<GetSummaryCommandResponse>(response);
    }
}