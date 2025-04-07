namespace CondominiumAlerts.Features.Features.Events;

public record EventsOfCondominiumDto(Guid Id, string Name, string Address, string ImageUrl, DateTime CreatedAt);