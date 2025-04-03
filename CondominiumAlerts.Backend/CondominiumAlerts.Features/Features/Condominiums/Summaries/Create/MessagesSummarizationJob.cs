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
    private readonly SummaryStatusService _summaryStatusService;
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
        IRepository<CondominiumUser, Guid> condominiumUserRepository,
        SummaryStatusService summaryStatusService)
    {
        _jobCancellationService = jobCancellationService;
        _logger = logger;
        _sender = sender;
        _hubContext = hubContext;
        _summaryRepository = summaryRepository;
        _userRepository = userRepository;
        _condominiumRepository = condominiumRepository;
        _condominiumUserRepository = condominiumUserRepository;
        _summaryStatusService = summaryStatusService;
    }
    
    public async Task Invoke()
    {
        await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Queued);
        await _hubContext.Clients.Group(Payload.CondominiumId.ToString()).SendAsync(
            "UpdateSummaryStatus", 
            _summaryStatusService.GetSummaryStatus(Payload.CondominiumId.ToString()), 
            CancellationToken);
        _jobId = Payload.JobId;
        CancellationToken = _jobCancellationService.GetCancellationToken(_jobId);

        if (CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"[Solicitud {_jobId}] Solicitud cancelada antes de comenzar.", _jobId);
            await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);
            await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                .SendAsync("CancelledProcessing", "Se cancelo el proceso antes de empezar", CancellationToken);
            return;
        }

        try
        {
            // Verificar cancelación justo antes de terminar
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);                
                await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                    .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.", CancellationToken);
                return;
            }
            //Validacion de condominio
            var condominium = await _condominiumRepository.GetByIdAsync(Payload.CondominiumId, CancellationToken);
            if (condominium is null)
            {
                var errorMessage = $"Condominio no encontrado. CondominiumId: {Payload.CondominiumId}";
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Failed);                
                _logger.LogWarning(errorMessage);
                return;
            }
            
            // Verificar cancelación justo antes de terminar
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);                
                await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                    .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.", CancellationToken);
                return;
            }
            // Validación de usuario
            var user = await _userRepository.GetByIdAsync(Payload.TriggeredBy, CancellationToken);
            if (user is null)
            {
                var errorMessage = $"Usuario no encontrado. UserId: {Payload.TriggeredBy}";
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Failed);                
                _logger.LogWarning(errorMessage);
                return;
            }
            
            
            // Verificar cancelación justo antes de terminar
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);                
                await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                    .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.", CancellationToken);
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
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Failed);                
                await _hubContext.Clients.Group(Payload.CondominiumId.ToString()).SendAsync("UserNotInCondominium", errorMessage, CancellationToken);
            }
            
            _logger.LogInformation($"[Solicitud {_jobId}] Procesando mensajes para el condominio [${condominium.Id} - ${condominium.Name}] por solicitud del usuario [${user.Id} - {user.Username}].");
            
            var command = new CreateSummaryCommand(condominium, user);
            // Verificar cancelación justo antes de terminar
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);                
                await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                    .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.", CancellationToken);
                return;
            }
            var result = await _sender.Send(command, CancellationToken);

            if (result.IsSuccess && result.Value is not null)
            {
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Completed);                
                await _hubContext.Clients.Group(command.Condominium.Id.ToString()).SendAsync("SendSummary", result.Value.Summary, CancellationToken);
                await _summaryRepository.CreateAsync(result.Value.Summary, CancellationToken);
            }
            else
            {
                // Notificar que hubo un error en el procesamiento
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Failed);
            }
            
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);
                await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                    .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.");
                return;
            }
            
            _logger.LogInformation($"[Solicitud {_jobId}] Resumen generado exitosamente.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"[Job {_jobId}] Se canceló correctamente.", _jobId);
            await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Cancelled);
            await _hubContext.Clients.Group(Payload.CondominiumId.ToString())
                .SendAsync("CancelledProcessing", "Se cancelo el proceso antes de empezar", CancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError($"[Job {_jobId}] Error: {ex.Message}.");
            await _summaryStatusService.SetSummaryStatus(Payload.CondominiumId.ToString(), SummaryStatus.Failed);
        }
        finally
        {
            _jobCancellationService.RemoveJob(_jobId);
            _summaryStatusService.CleanupCompletedStatuses(Payload.CondominiumId.ToString());
            await _hubContext.Clients.Group(Payload.CondominiumId.ToString()).SendAsync("ProcessingComplete", "Proceso completado con éxito", CancellationToken);
        }
    }
    
}