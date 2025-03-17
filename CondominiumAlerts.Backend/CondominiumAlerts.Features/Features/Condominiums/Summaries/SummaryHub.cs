using CondominiumAlerts.Domain.Aggregates.Entities;
using Microsoft.AspNetCore.SignalR;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public class SummaryHub : Hub
{
    
    //Methods:
    //SendSummary
    //NotifyProcessingStarted
    //NotifyProcessingFinished
    //NotifyProcessingError
    //Metodo para enviar el resumen al cliente despues de haberlo procesado

    //Metodo para unir un condominio al hub
    public async Task JoinGroup(string condominiumId, string condominiumName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, condominiumId);
        await Clients.Group(condominiumId).SendAsync("NotifyProcessingStarted",
            $"El procesamiento para el resumen del condominio ${condominiumName} ha empezado.");
    }
    
    //Metodo para sacar un condominio del hub
    public async Task LeaveGroup(string condominiumId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, condominiumId);
        await Clients.Group(condominiumId).SendAsync("NotifyProcessingStarted",
            $"El procesamiento para el resumen del condominio ${condominiumId} finalizo exitosamente.");
    }

    public async Task SendSummary(string condominiumId, string summary)
    {
        await Clients.Group(condominiumId).SendAsync("SendSummary", summary);
    }
}