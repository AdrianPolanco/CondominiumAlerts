using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Services.Cancellation;
using LightResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries.Cancel;

public class CancelSummaryCommandHandler : ICommandHandler<CancelSummaryCommand, Result<CancelSummaryResponse>>
{
    private readonly JobCancellationService _jobCancellationService;
    private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
    private readonly ILogger<CancelSummaryCommandHandler> _logger;
    private readonly IHubContext<SummaryHub> _hubContext;

    public CancelSummaryCommandHandler(
        JobCancellationService jobCancellationService, 
        IRepository<CondominiumUser, Guid> condominiumUserRepository,
        ILogger<CancelSummaryCommandHandler> logger,
        IHubContext<SummaryHub> hubContext
        )
    {
        _jobCancellationService = jobCancellationService;
        _condominiumUserRepository = condominiumUserRepository;
        _logger = logger;
        _hubContext = hubContext;
    }
    
    public async Task<Result<CancelSummaryResponse>> Handle(CancelSummaryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[RECEIVED] Recibiendo solicitud de cancelacion para el trabajo {jobId} en el condominio {condominiumId} por parte del usuario {userId}",
            request.JobId,
            request.CondominiumId,
            request.UserId);
        // 1. Verificar si el usuario está asociado al condominio
        var condominiumUser = await _condominiumUserRepository.GetAsync(
            cancellationToken: cancellationToken,
            filter: cu => cu.UserId == request.UserId && cu.CondominiumId == request.CondominiumId);

        if (condominiumUser is null)
        {
            _logger.LogError("[USER-NOT-AUTORIZED] El usuario {userId} no esta autorizado." , request.UserId);
            // Si el usuario no pertenece al condominio, denegar la cancelación
            return Result<CancelSummaryResponse>.Fail("El usuario no está autorizado a cancelar este proceso en este condominio.");
        }

        _logger.LogInformation("[USER-AUTORIZED] El usuario {userId} esta autorizado a cancelar el job {jobId}", request.UserId, request.JobId);
        // 2. Intentar cancelar el trabajo
        bool cancelled = _jobCancellationService.CancelJob(request.JobId);
        _logger.LogInformation("Estado de los trabajos registrados: {0}", _jobCancellationService.GetAllJobs());
        if (cancelled)
        {
            _logger.LogInformation("[SUMMARY-CANCELLED] El resumen {jobId} fue cancelado exitosamente", request.JobId);
            await _hubContext.Clients.Group(request.CondominiumId.ToString())
                .SendAsync("CancelledProcessing", "Se ha cancelado el proceso.", cancellationToken);
            // Si la cancelación fue exitosa, retornar éxito
            return Result<CancelSummaryResponse>.Ok(
                new CancelSummaryResponse( 
                    true, 
                    "El resumen fue cancelado exitosamente", 
                    request.CondominiumId, 
                    request.JobId, 
                    request.UserId));
        }
        else
        {
            _logger.LogError("[JOB-NOT-FOUND] El job {jobId} no fue encontrado", request.JobId);
            // Si no se encontró el trabajo para cancelar, retornar error
            return Result<CancelSummaryResponse>.Fail("No se encontró el trabajo para cancelar.");
        }
    }
}