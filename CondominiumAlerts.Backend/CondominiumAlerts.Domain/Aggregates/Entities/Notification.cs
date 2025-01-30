
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class Notification : IEntity<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty!;
        public string? Description { get; set; } 
        // if receiver user id is null means that the notification is global 
        public Guid? ReceiverUserId { get; set; }
        public int CondominiumId { get; set; }
        public int LevelOfPriorityId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }


        [ForeignKey("ReceiverUserId")]
        public User? ReceiverUser { get; set; }
        [ForeignKey("CondominiumId")]
        public Condominium? Condominium { get; set; }
        [ForeignKey("LevelOfPriorityId")]
        public LevelOfPriority LevelOfPriority { get; set; }

    }
}
