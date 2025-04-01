using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries.Get;

public record GetSummaryQuery(string UserId, Guid CondominiumId): IQuery<Result<GetSummaryResponse>>;