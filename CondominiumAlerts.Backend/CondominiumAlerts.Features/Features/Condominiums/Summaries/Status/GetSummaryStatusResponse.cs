namespace CondominiumAlerts.Features.Features.Condominiums.Summaries.Status;

public record GetSummaryStatusResponse(Guid CondominiumId, SummaryStatus? Status);