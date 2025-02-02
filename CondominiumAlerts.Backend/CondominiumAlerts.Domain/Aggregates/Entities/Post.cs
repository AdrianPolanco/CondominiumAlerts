using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class Post : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty!;
        public string Description { get; set; } = string.Empty!;
        public string ImageUrl { get; set; } = string.Empty!;
        public Guid CondominiumId { get; set; }
        public string UserId { get; init; }
        public Guid LevelOfPriorityId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        
        public Condominium Condominium { get; set; }
        public User User { get; set; }
        public LevelOfPriority LevelOfPriority { get; set; }

        public IReadOnlyCollection<Comment>? Comments { get; set; }
    }
