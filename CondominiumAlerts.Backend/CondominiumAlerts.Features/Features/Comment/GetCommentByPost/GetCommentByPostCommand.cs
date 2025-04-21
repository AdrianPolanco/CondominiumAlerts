using CondominiumAlerts.Features.Features.Posts.Get;
using LightResults;
using MediatR;

namespace CondominiumAlerts.Features.Features.Comment.GetCommentByPost
{
    public class GetCommentByPostCommand : IRequest<Result<List<GetCommentByPostResponse>>>
    {
        public Guid PostId { get; init; }
    }
}
