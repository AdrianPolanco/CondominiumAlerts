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
    public async Task JoinGroup(string condominiumId, string condominiumName, string username)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, condominiumId);
        await Clients.Group(condominiumId).SendAsync("NotifyProcessingStarted",
            $"El procesamiento para el resumen del condominio ${condominiumName} ha empezado a solicitud de ${username}.");
    }
    
    //Metodo para sacar un condominio del hub
    public async Task LeaveGroup(string condominiumId, string condominiumName, string username)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, condominiumId);
        await Clients.Group(condominiumId).SendAsync("NotifyProcessingStarted",
            $"El procesamiento para el resumen del condominio ${condominiumName} solicitado por ${username} finalizo exitosamente.");
    }
}