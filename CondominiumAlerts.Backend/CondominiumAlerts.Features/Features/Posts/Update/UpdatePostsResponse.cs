namespace CondominiumAlerts.Features.Features.Posts.Update
{
    public class UpdatePostsResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid LevelOfPriorityId { get; set; }
    }
}