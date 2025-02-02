
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class CondominiumUser : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid CondominiumId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        
        public Condominium Condominium { get; set; }
        public User User { get; set; }

    }
