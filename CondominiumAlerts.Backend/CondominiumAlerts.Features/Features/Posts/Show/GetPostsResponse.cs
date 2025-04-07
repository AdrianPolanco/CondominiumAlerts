namespace CondominiumAlerts.Features.Features.Posts.Get
{
    public class GetPostsResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CondominiumId { get; set; }
        public string UserId { get; set; }
        public Guid LevelOfPriorityId { get; set; }
    }
}