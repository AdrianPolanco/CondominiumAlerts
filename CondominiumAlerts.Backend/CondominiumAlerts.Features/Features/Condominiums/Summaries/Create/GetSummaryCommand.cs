using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using CondominiumAlerts.Domain.Aggregates.Entities;
using LightResults;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public record GetSummaryCommand(Condominium Condominium, User TriggeredByUser): ICommand<Result<GetSummaryCommandResponse>>;