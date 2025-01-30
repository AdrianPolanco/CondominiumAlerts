
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class Message : IEntity<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text{ get; set; } = string.Empty!;
        public string? MediaUrl { get; set; }
        public Guid CreatorUserId { get; set; }
        // if receiver user id is null means that the message is for the global chat
        public Guid? ReceiverUserId { get; set; }
        public int CondominiumId { get; set; }
        public int? MessageBeingRepliedToId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }


        [ForeignKey("CreatorUserId")]
        public User CreatorUser { get; set; }
        [ForeignKey("ReceiverUserId")]
        public User? ReceiverUser { get; set; }
        [ForeignKey("CondominiumId")]
        public Condominium Condominium { get; set; }
        [ForeignKey("CreatorUserId")]
        public Message? MessageBeingRepliedTo { get; set; }

    }
}
