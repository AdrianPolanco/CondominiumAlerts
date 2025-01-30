
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class CondominiumUser : IEntity<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CondominiumId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }


        [ForeignKey("CondominiumId")]
        public Condominium Condominium { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
