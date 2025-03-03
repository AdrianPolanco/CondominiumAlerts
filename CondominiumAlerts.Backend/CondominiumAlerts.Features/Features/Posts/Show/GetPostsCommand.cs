using LightResults;
using MediatR;

namespace CondominiumAlerts.Features.Features.Posts.Get
{
	public class GetPostsCommand : IRequest<Result<List<GetPostsResponse>>>
	{
		
	}
}