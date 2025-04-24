using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Comment.Delete
{
    public class DeleteCommentCommand : ICommand<Result<DeleteCommentResponse>>
    {
         public Guid Id { get; set; }
         public Guid PostId { get; set; }
    }
}
