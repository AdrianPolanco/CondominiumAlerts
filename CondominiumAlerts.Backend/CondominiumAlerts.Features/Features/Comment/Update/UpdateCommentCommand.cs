using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Comment.Update
{
    public class UpdateCommentCommand : ICommand<Result<UpdateCommentResponse>>
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty!;
        public IFormFile ImageFile { get; set; }
        public bool RemoveImage { get; set; }
    }
}