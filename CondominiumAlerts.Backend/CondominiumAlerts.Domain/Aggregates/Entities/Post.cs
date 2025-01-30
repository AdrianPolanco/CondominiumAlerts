using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class Post : IEntity<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty!;
        public string Description { get; set; } = string.Empty!;
        public string ImageUrl { get; set; } = string.Empty!;
        public int CondominiumId { get; set; }
        public Guid UserId { get; set; }
        public int LevelOfPriorityId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }


        [ForeignKey("CondominiumId")]
        public Condominium Condominium { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("LevelOfPriorityId")]
        public LevelOfPriority LevelOfPriority { get; set; }

        public IReadOnlyCollection<Comment>? Comments { get; set; }
    }
}
