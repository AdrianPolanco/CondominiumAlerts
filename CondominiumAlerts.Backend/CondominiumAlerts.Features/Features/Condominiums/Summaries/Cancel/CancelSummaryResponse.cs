namespace CondominiumAlerts.Features.Features.Condominiums.Summaries.Cancel;

public record CancelSummaryResponse(bool Cancelled, string Message, Guid CondominiumId, Guid JobId, string RequestedByUserId);