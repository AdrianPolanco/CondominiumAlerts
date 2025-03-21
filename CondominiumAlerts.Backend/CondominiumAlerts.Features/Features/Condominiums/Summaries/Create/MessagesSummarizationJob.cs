using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Features.Users.Register;
using CondominiumAlerts.Infrastructure.Services.Cancellation;
using Coravel.Invocable;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public class MessagesSummarizationJob : IInvocable, IInvocableWithPayload<MessagesSummarizationRequest>, ICancellableInvocable
{    
    private Guid _jobId;
    private readonly JobCancellationService _jobCancellationService;
    private readonly ILogger<MessagesSummarizationJob> _logger;
    private readonly IRepository<Summary, Guid> _summaryRepository;
    private readonly ISender _sender;
    private readonly IHubContext<SummaryHub> _hubContext;
    private readonly IRepository<User, string> _userRepository;
    private readonly IRepository<Condominium, Guid> _condominiumRepository;
    private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
    public MessagesSummarizationRequest Payload { get; set; }
    public CancellationToken CancellationToken { get; set; }
    
    public MessagesSummarizationJob(
        JobCancellationService jobCancellationService, 
        ILogger<MessagesSummarizationJob> logger, 
        ISender sender, 
        IHubContext<SummaryHub> hubContext,
        IRepository<Summary, Guid> summaryRepository,
        IRepository<User, string> userRepository,
        IRepository<Condominium, Guid> condominiumRepository,
        IRepository<CondominiumUser, Guid> condominiumUserRepository)
    {
        _jobCancellationService = jobCancellationService;
        _logger = logger;
        _sender = sender;
        _hubContext = hubContext;
        _summaryRepository = summaryRepository;
        _userRepository = userRepository;
        _condominiumRepository = condominiumRepository;
        _condominiumUserRepository = condominiumUserRepository;
    }
    
    public async Task Invoke()
    {
        _jobId = Payload.JobId;
        CancellationToken = _jobCancellationService.GetCancellationToken(_jobId);

        if (CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"[Solicitud {_jobId}] Solicitud cancelada antes de comenzar.", _jobId);
            _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                .SendAsync("CancelledProcessing", "Se cancelo el proceso antes de empezar", CancellationToken);
        }

        try
        {
            //Validacion de condominio
            var condominium = await _condominiumRepository.GetByIdAsync(Payload.CondominiumId, CancellationToken);
            if (condominium is null)
            {
                var errorMessage = $"Condominio no encontrado. CondominiumId: {Payload.CondominiumId}";
                _logger.LogWarning(errorMessage);
                return;
            }
            
            // Validación de usuario
            var user = await _userRepository.GetByIdAsync(Payload.TriggeredBy, CancellationToken);
            if (user is null)
            {
                var errorMessage = $"Usuario no encontrado. UserId: {Payload.TriggeredBy}";
                _logger.LogWarning(errorMessage);
                return;
            }
            
            //Validando que el usuario efectivamente esta en el condominio
            var condominiumUser = await _condominiumUserRepository.GetAsync(
                cancellationToken: CancellationToken,
                filter: cu => cu.UserId == user.Id && cu.CondominiumId == condominium.Id
                );


            if (condominiumUser is null)
            {
                var errorMessage = $"El usuario ${user.Username} no pertenece al condominio ${condominium.Name}.";
                _logger.LogWarning(errorMessage);
                _hubContext.Clients.Group(Payload.CondominiumId.ToString()).SendAsync("UserNotInCondominium", errorMessage);
            }
            
            _logger.LogInformation($"[Solicitud {_jobId}] Procesando mensajes para el condominio [${condominium.Id} - ${condominium.Name}] por solicitud del usuario [${user.Id} - {user.Username}].");
            
            var command = new GetSummaryCommand(condominium, user);
            var result = await _sender.Send(command, CancellationToken);

            if (result.IsSuccess && result.Value is not null)
            {
                await _hubContext.Clients.Group(command.Condominium.Id.ToString()).SendAsync("SendSummary", result.Value.Summary, CancellationToken);
               // await _summaryRepository.CreateAsync(result.Value.Summary, CancellationToken);
            }
            else
            {
                // Notificar que hubo un error en el procesamiento
                await _hubContext.Clients.Group(command.Condominium.Id.ToString())
                    .SendAsync("NotifyProcessingError", $"Error en el procesamiento: {result.Error}", CancellationToken);
            }
            
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                    .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.");
                return;
            }
            
            _logger.LogInformation($"[Solicitud {_jobId}] Resumen generado exitosamente.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"[Job {_jobId}] Se canceló correctamente.", _jobId);
            _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                .SendAsync("CancelledProcessing", "Se cancelo el proceso antes de empezar", CancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError($"[Job {_jobId}] Error: {ex.Message}.");
            await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                .SendAsync("NotifyProcessingError", $"Error en el procesamiento: {ex.Message}", CancellationToken);
        }
        finally
        {
            _jobCancellationService.RemoveJob(_jobId);
            await _hubContext.Clients.Group(Payload.CondominiumId.ToString()).SendAsync("ProcessingComplete", "Proceso completado con éxito");
        }
    }
    
}