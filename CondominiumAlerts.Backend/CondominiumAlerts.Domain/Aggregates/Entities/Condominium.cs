
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class Condominium : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty!;
        public string Address { get; set; } = string.Empty!;
        public string ImageUrl { get; set; } = string.Empty!;
        public string LinkToken { get; set; } = string.Empty!;
        public string InviteCode { get; set; } = string.Empty!;
        public DateTime TokenExpirationDate {  get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }


        public IReadOnlyCollection<User>? Users { get; set; }
        public IReadOnlyCollection<Post>? Posts { get; set; }
        public IReadOnlyCollection<LevelOfPriority>? LevelOfPriorities { get; set; }
        public IReadOnlyCollection<Message>? Messages { get; set; }

    }
