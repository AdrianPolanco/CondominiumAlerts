using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Services;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using LightResults;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
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
    private readonly SummaryStatusService _summaryStatusService;

    public CreateSummaryCommandHandler(
        ILogger<CreateSummaryCommandHandler> logger, 
        IRepository<Message, Guid> messageRepository,
        IAiService aiService,
        IHubContext<SummaryHub> hubContext,
        SummaryStatusService summaryStatusService
        )
    {
        _logger = logger;
        _messageRepository = messageRepository;
        _aiService = aiService;
        _hubContext = hubContext;
        _summaryStatusService = summaryStatusService;
    }
    
    public async Task<Result<CreateSummaryCommandResponse>> Handle(CreateSummaryCommand request, CancellationToken cancellationToken)
    {
        await _summaryStatusService.SetSummaryStatus(request.Condominium.Id.ToString(), SummaryStatus.Processing); 
        
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
            await _summaryStatusService.SetSummaryStatus(request.Condominium.Id.ToString(), SummaryStatus.Failed);
            var errorMessage = $"No se encontraron mensajes en el condominio. CondominiumId: {request.Condominium.Id}, Mensajes: 0";
            _logger.LogWarning(errorMessage);
            await _hubContext.Clients.Group(request.Condominium.Id.ToString()).SendAsync("NoNewMessages", "No se encontraron nuevos mensajes en las ultimas 24 horas.", cancellationToken);
            return Result.Fail<CreateSummaryCommandResponse>(errorMessage);
        }
        
        
        var recoveredMessagesLog = $"Mensajes recuperados exitosamente. CondominiumId: {request.Condominium.Id}. Cantidad de mensajes: {messages.Count}";
        _logger.LogInformation(recoveredMessagesLog);
        
        var messagesDto = messages.Adapt<List<MessageSummaryDto>>();
        
        _logger.LogInformation("MessagesDTO: {dto}", messagesDto);
        
        if (cancellationToken.IsCancellationRequested)
        {
            await _summaryStatusService.SetSummaryStatus(request.Condominium.Id.ToString(), SummaryStatus.Cancelled);
            _logger.LogInformation(cancellationMessage);
            return Result<CreateSummaryCommandResponse>.Fail(cancellationMessage);
        }

        var messagesDtoWithUserInfo = messagesDto.Select(m =>
        {
            return new MessageSummaryDto(
                Id: m.Id,
                Text: m.Text,
                CondominiumId: m.CondominiumId,
                Condominium: m.Condominium,
                CreatorUserId: m.CreatorUserId,
                CreatorUser: m.CreatorUser,
                CreatorUsername: m.CreatorUser.Username,
                CreatorUserFullname: $"{m.CreatorUser.Name} {m.CreatorUser.Lastname}",
                CreatedAt: m.CreatedAt
            );
        }).ToList();

        var summary = await _aiService.GenerateSummary(messagesDtoWithUserInfo, request.TriggeredByUser, request.Condominium, cancellationToken);
        
        if(summary is null)
        {
            await _summaryStatusService.SetSummaryStatus(request.Condominium.Id.ToString(), SummaryStatus.Failed);
            return Result.Fail<CreateSummaryCommandResponse>("Hubo un error al resumir la conversacion.");
        }
        var response = new CreateSummaryCommandResponse(summary);

       
        var successMessage = $"Resumen solicitado exitosamente. CondominiumId: {request.Condominium.Id}, UserId: {request.TriggeredByUser}, Mensajes: {messages.Count}";
        _logger.LogInformation(successMessage);
        
        return Result.Ok<CreateSummaryCommandResponse>(response);
    }
}