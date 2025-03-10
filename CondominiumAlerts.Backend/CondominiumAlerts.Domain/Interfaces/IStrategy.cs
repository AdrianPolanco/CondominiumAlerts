namespace CondominiumAlerts.Domain.Interfaces;

public interface IStrategy<TInput, TOutput>
{
    bool CanHandle(TInput input);
    Task<TOutput> HandleAsync(TInput input, CancellationToken cancellationToken);
}