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

public class CreateSummaryCommandHandler : ICommandHandler<CreateSummaryCommand, Result<CreateSummaryCommandResponse>>
{
    private readonly IHubContext<SummaryHub> _hubContext;
    private readonly ILogger<CreateSummaryCommandHandler> _logger;
    private readonly IRepository<Message, Guid> _messageRepository;
    private readonly IAiService _aiService;

    public CreateSummaryCommandHandler(
        ILogger<CreateSummaryCommandHandler> logger, 
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
    
    public async Task<Result<CreateSummaryCommandResponse>> Handle(CreateSummaryCommand request, CancellationToken cancellationToken)
    {
        var cancellationMessage = "Procesamiento de resumenes cancelado...";

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(cancellationMessage);
            return Result<CreateSummaryCommandResponse>.Fail(cancellationMessage);
        }
        // Validación de mensajes
        var messages = await _messageRepository.GetAsync(filter: m => m.CondominiumId == request.Condominium.Id /*&& m.CreatedAt > DateTime.UtcNow.AddHours(-24)*/, cancellationToken: cancellationToken);
        if (messages.Count == 0)
        {
            var errorMessage = $"No se encontraron mensajes en el condominio. CondominiumId: {request.Condominium.Id}, Mensajes: 0";
            _logger.LogWarning(errorMessage);
            _hubContext.Clients.Group(request.Condominium.Id.ToString()).SendAsync("NoNewMessages", "No se encontraron nuevos mensajes en las ultimas 24 horas.", cancellationToken);
            return Result.Fail<CreateSummaryCommandResponse>(errorMessage);
        }
        
        
        var recoveredMessagesLog = $"Mensajes recuperados exitosamente. CondominiumId: {request.Condominium.Id}. Cantidad de mensajes: {messages.Count}";
        _logger.LogInformation(recoveredMessagesLog);
        
        var messagesDto = messages.Adapt<List<MessageSummaryDto>>();
        
        _logger.LogInformation("MessagesDTO: {dto}", messagesDto);
        
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(cancellationMessage);
            return Result<CreateSummaryCommandResponse>.Fail(cancellationMessage);
        }

        var summary = await _aiService.GenerateSummary(messagesDto, request.TriggeredByUser, request.Condominium, cancellationToken);
        
        if(summary is null) return Result.Fail<CreateSummaryCommandResponse>("Hubo un error al resumir la conversacion.");
        var response = new CreateSummaryCommandResponse(summary);

      /* var testSummary = new Summary()
       {
           CondominiumId = request.Condominium.Id,
           Condominium = request.Condominium,
           Content = "ESTE ES UN MENSAJE DE PRUEBA PARA SIGNALR",
           CreatedAt = DateTime.UtcNow,
           Id = Guid.NewGuid(),
           TriggeredBy = request.TriggeredByUser.Id,
           User = request.TriggeredByUser
       };
       var response = new CreateSummaryCommandResponse(testSummary);*/
       
        var successMessage = $"Resumen solicitado exitosamente. CondominiumId: {request.Condominium.Id}, UserId: {request.TriggeredByUser}, Mensajes: {messages.Count}";
        _logger.LogInformation(successMessage);
        
        return Result.Ok<CreateSummaryCommandResponse>(response);
    }
}