using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Comment.Update
{
    public class UpdateCommentResponse
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty!;
        public string ImageUrl { get; set; }
    }
}
