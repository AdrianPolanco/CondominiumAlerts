
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities;
public sealed class CondominiumUser : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid CondominiumId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Condominium Condominium { get; set; }
    public User User { get; set; }

}
