using System.Collections.Concurrent;

namespace CondominiumAlerts.Infrastructure.Services.Cancellation;

public class JobCancellationService
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _cancellations = new();

    // Registrar el job en el JobCancellationService
    public Guid RegisterJob()
    {
        var jobId = Guid.NewGuid();
        _cancellations[jobId] = new CancellationTokenSource();
        _cancellations.TryAdd(jobId, _cancellations[jobId]);
        return jobId;
    }

    // Obtener token de cancelacion
    public CancellationToken GetCancellationToken(Guid jobId)
    {
        return _cancellations.TryGetValue(jobId, out var tokenSource) ? tokenSource.Token : CancellationToken.None;
    }

    // Cancelar Job
    public bool CancelJob(Guid jobId)
    {
        if (_cancellations.TryGetValue(jobId, out var tokenSource))
        {
            tokenSource.Cancel();
            return true;
        }
        return false;
    }
    
    public IEnumerable<KeyValuePair<Guid, CancellationTokenSource>> GetAllJobs()
    {
        return _cancellations;
    }


    public bool RemoveJob(Guid jobId)
    {
        if (_cancellations.TryRemove(jobId, out _))
        {
            return true;
        }
        
        return false;
    }
}