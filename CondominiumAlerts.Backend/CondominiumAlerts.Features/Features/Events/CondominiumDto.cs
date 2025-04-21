namespace CondominiumAlerts.Features.Features.Events;

public record CondominiumDto(Guid Id, string Name, string Address, string ImageUrl, DateTime CreatedAt);