using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries.Cancel;

public record CancelSummaryCommand(Guid CondominiumId, string UserId, Guid JobId): ICommand<Result<CancelSummaryResponse>>;