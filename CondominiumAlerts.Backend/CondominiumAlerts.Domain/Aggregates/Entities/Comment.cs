
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace CondominiumAlerts.Domain.Aggregates.Entities;
    public sealed class Comment : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty!;
        public string ImageUrl { get; set; } = string.Empty!;
        public Guid PostId { get; set; }
        public string UserId { get; set; }
        // if it has a parent comment id it is a comment repling to another comment
        public Guid? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
        public Post Post { get; init; }
        public Comment? ParentComment { get; set; }
        public User User { get; init; }
        public IReadOnlyCollection<Comment>? Replies { get; set; }
    }

