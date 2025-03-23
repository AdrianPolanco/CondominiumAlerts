

using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public class SummaryStatusService
{
    private readonly ConcurrentDictionary<string, SummaryStatus> _summaries = new();
    private readonly IHubContext<SummaryHub> _hubContext;

    public SummaryStatusService(IHubContext<SummaryHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task SetSummaryStatus(string key, SummaryStatus summaryStatus)
    {
        _summaries[key] = summaryStatus;
        await _hubContext.Clients.Group(key).SendAsync("UpdateSummaryStatus", summaryStatus);
    }

    public SummaryStatus? GetSummaryStatus(string key)
    {
        return _summaries.TryGetValue(key, out var summaryStatus) ? summaryStatus : null;
    }
    
    // Método para verificar si existe un procesamiento activo
    public bool IsProcessingActive(string key)
    {
        return _summaries.TryGetValue(key, out var status) && 
               (status == SummaryStatus.Queued || status == SummaryStatus.Processing);
    }
    
    // Método para limpiar estados completados o fallidos después de cierto tiempo
    public void CleanupCompletedStatuses(string key)
    {
        if (_summaries.TryGetValue(key, out var status) &&
            (status == SummaryStatus.Completed || status == SummaryStatus.Failed || status == SummaryStatus.Cancelled))
        {
            _summaries.TryRemove(key, out _);
        }
    }
}