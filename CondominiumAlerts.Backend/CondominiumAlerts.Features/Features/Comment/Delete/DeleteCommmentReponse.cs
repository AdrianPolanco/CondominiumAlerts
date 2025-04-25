using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Comment.Delete
{
    public record DeleteCommentResponse(
        Guid Id,
        string Text,
        string? ImageUrl,
        Guid PostId,
        string UserId,
        DateTime UpdatedAt
    )
    {
        public static implicit operator DeleteCommentResponse(Domain.Aggregates.Entities.Comment  comment) => new(
            comment.Id,
            comment.Text,
            comment.ImageUrl,
            comment.PostId,
            comment.UserId,
            comment.UpdatedAt
        );
    }
}
