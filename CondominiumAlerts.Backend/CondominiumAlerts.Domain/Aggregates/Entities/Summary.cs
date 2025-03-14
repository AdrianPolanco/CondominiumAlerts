using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Features.Features.Summary;

public class Summary
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string TriggeredBy { get; set; }
    public User User { get; set; }
    public Guid CondominiumId { get; set; }
    public Condominium Condominium { get; set; }
    public DateTime CreatedAt { get; set; }
}