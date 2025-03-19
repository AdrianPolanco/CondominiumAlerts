using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Messages.Condominiums;

public record GetMessagesInCondominiumQuery(Guid CondominiumId) : IQuery<Result<List<ChatMessageDto>>>;