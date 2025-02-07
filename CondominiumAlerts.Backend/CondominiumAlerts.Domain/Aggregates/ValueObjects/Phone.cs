namespace CondominiumAlerts.Domain.Aggregates.ValueObjects;

public record Phone
{
    public string Number { get; set; } = string.Empty;
}