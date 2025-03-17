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
    public MessagesSummarizationRequest Payload { get; set; }
    public CancellationToken CancellationToken { get; set; }
    
    public MessagesSummarizationJob(JobCancellationService jobCancellationService, ILogger<MessagesSummarizationJob> logger, ISender sender, IHubContext<SummaryHub> hubContext)
    {
        _jobCancellationService = jobCancellationService;
        _logger = logger;
        _sender = sender;
        _hubContext = hubContext;
    }
    
    public async Task Invoke()
    {
        _jobId = _jobCancellationService.RegisterJob();
        CancellationToken = _jobCancellationService.GetCancellationToken(_jobId);

        if (CancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"[Solicitud {_jobId}] Solicitud cancelada antes de comenzar.", _jobId);
        }

        try
        {
            _logger.LogInformation($"[Solicitud {_jobId}] Procesando mensajes para el condominio [${Payload.CondominiumId} - ${Payload.CondominiumName}].");
            
            var command = Payload.Adapt<GetSummaryCommand>();
            var result = await _sender.Send(command, CancellationToken);

            if (result.IsSuccess && result.Value is GetSummaryCommandResponse response)
            {
                await _hubContext.Clients.Group(command.CondominiumId.ToString()).SendAsync("SendSummary", response.Summary.Content);
                
            }
            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[Solicitud {_jobId}] Cancelando solicitud...");
                return;
            }
            
            _logger.LogInformation($"[Solicitud {_jobId}] Resumen generado exitosamente.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"[Job {_jobId}] Se canceló correctamente.", _jobId);
        }
        catch(Exception ex)
        {
            _logger.LogError($"[Job {_jobId}] Error: {ex.Message}.");
        }
        finally
        {
            _jobCancellationService.RemoveJob(_jobId);
        }
    }


}