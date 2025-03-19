using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;
using OpenAI.Chat;

namespace CondominiumAlerts.Infrastructure.Services.AI.MessagesSummary;

public interface IAiService
{
    Task<Summary?> GenerateSummary(List<MessageSummaryDto> messages, User user, Condominium condominium, CancellationToken cancellationToken);
}