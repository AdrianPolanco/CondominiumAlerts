using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Aggregates.Entities;

public class Summary : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string TriggeredBy { get; set; }
    public User User { get; set; }
    public Guid CondominiumId { get; set; }
    public Condominium Condominium { get; set; }
    public DateTime CreatedAt { get; set; }
}