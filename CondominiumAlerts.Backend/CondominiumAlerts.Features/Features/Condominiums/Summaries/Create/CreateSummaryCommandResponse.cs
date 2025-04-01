using CondominiumAlerts.Domain.Aggregates.Entities;
using OpenAI.Chat;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public record CreateSummaryCommandResponse(Summary Summary);