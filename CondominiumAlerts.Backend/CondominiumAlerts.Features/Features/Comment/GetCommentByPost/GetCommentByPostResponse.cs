using CloudinaryDotNet.Actions;

namespace CondominiumAlerts.Features.Features.Comment.GetCommentByPost
{
    public class GetCommentByPostResponse
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty!;
        public string ImageUrl { get; set; } = string.Empty!;
        public Guid PostId { get; set; }
        public string UserId { get; set; }
        public string UserProfilePicture { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
