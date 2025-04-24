using CondominiumAlerts.Domain.Aggregates.Entities;
namespace CondominiumAlerts.Features.Features.Posts.Delete
{
    public record DeletePostResponse(
        Guid Id,
        string Title,
        Guid PriorityId,
        string Description,
        Guid? CondominiumId,
        DateTime UpdatedAt
    )

    {

        public static implicit operator DeletePostResponse(Post post) => new(
          post.Id,
          post.Title,
          post.LevelOfPriorityId,
          post.Description,
          post.CondominiumId,
          post.UpdatedAt
        );

    };
}
