using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Features.Features.Users.Register;

namespace CondominiumAlerts.Features.Features.Condominiums.Summaries;

public record MessagesSummarizationRequest(Guid CondominiumId, string TriggeredBy, Guid JobId);