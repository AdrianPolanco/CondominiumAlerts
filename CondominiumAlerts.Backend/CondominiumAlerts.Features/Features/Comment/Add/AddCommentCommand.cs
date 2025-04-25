using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Comment.Add
{
    public class AddCommentCommand : ICommand<Result<AddCommentResponse>>
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public IFormFile ImageFile { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; }
    }
}
