using LightResults;
using MediatR;
using LightResults;

namespace CondominiumAlerts.Features.Features.Posts.Get
{
    public class GetPostByIdCommand : IRequest<Result<GetPostByIdResponse>>
    {
        public Guid Id { get; init; }
    }
}