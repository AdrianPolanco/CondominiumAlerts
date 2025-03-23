using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public class SummaryHub : Hub
{
    private readonly SummaryStatusService _summaryStatusService;

    public SummaryHub(SummaryStatusService summaryStatusService)
    {
        _summaryStatusService = summaryStatusService;
    }
    //Methods:
    //CancelledProcessing
    //SendSummary
    //NotifyProcessingError
    //UserNotInCondominium
    
    //Metodo para unir un condominio al hub
    public async Task JoinGroup(string condominiumId, string condominiumName, string username)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, condominiumId);
        var currentStatus = _summaryStatusService.GetSummaryStatus(condominiumId);
        if (currentStatus.HasValue) await Clients.Caller.SendAsync("UpdateSummaryStatus", currentStatus.Value);
        await Clients.Group(condominiumId).SendAsync("NotifyProcessingStarted",
            $"El procesamiento para el resumen del condominio ${condominiumName} ha empezado a solicitud de ${username}.");
    }
    
    //Metodo para sacar un condominio del hub
    public async Task LeaveGroup(string condominiumId, string condominiumName, string username)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, condominiumId);
        await Clients.Group(condominiumId).SendAsync("NotifyProcessingSuccess",
            $"El procesamiento para el resumen del condominio ${condominiumName} solicitado por ${username} finalizo exitosamente.");
    }
}