using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public record GetSummaryCommand(Guid CondominiumId, string TriggeredBy): ICommand<Result<GetSummaryCommandResponse>>;