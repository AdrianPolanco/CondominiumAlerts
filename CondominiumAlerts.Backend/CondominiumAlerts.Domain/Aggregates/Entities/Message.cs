
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class Message : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty!;
        public string? MediaUrl { get; set; }
        public string CreatorUserId { get; set; }
        // if receiver user id is null means that the message is for the global chat
        public string? ReceiverUserId { get; set; }
        public Guid? CondominiumId { get; set; }
        public Guid? MessageBeingRepliedToId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User CreatorUser { get; set; }
        public User? ReceiverUser { get; set; }
        public Condominium Condominium { get; set; }
        public Message? MessageBeingRepliedTo { get; set; }

    }
