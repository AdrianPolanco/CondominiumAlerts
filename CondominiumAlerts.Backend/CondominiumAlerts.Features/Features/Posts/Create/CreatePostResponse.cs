using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Features.Features.Posts.Create
{
    public class CreatePostResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CondominiumId { get; set; }
        public string UserId { get; set; }
        public Guid LevelOfPriorityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public LevelOfPriority LevelOfPriority { get; set; }
    }
}