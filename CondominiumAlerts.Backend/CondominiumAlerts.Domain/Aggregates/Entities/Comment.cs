
using CondominiumAlerts.Domain.Aggregates.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;


namespace CondominiumAlerts.Domain.Aggregates.Entities
{
    public sealed class Comment : IEntity<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty!;
        public string ImageUrl { get; set; } = string.Empty!;
        public int PostId { get; set; }
        // if it has a parent comment id it is a comment repling to another comment
        public int? ParrentCommentId { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }


        [ForeignKey("PostId")]
        public Post Post { get; set; }
        [ForeignKey("ParrentCommentId")]
        public Comment? ParentComment { get; set; }

        public IReadOnlyCollection<Comment>? Replies { get; set; }
    }
}
