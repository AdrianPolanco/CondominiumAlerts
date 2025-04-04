using CondominiumAlerts.Domain.Aggregates.Interfaces;

namespace CondominiumAlerts.Domain.Aggregates.Entities;

public class Event : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsStarted { get; set; } = false;
    public bool IsFinished { get; set; } = false;
    public bool IsToday { get; set; }
    public User CreatedBy { get; set; }
    public string CreatedById { get; set; }
    public List<User> Suscribers { get; set; } = new();
    public Guid CondominiumId { get; set; }
    public Condominium Condominium { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}