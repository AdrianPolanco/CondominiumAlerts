
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class Notification : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty!;
        public string? Description { get; set; } 
        // if receiver user id is null means that the notification is global 
        public string? ReceiverUserId { get; set; }
        public Guid CondominiumId { get; set; }
        public Guid LevelOfPriorityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User? ReceiverUser { get; set; }
        public Condominium? Condominium { get; set; }
        public LevelOfPriority LevelOfPriority { get; set; }

    }

