using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using LightResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CondominiumAlerts.Features.Features.Posts.Get
{
    public class GetPostsHandler : IRequestHandler<GetPostsCommand, Result<List<GetPostsResponse>>>
    {
        private readonly ApplicationDbContext _context;

        public GetPostsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<GetPostsResponse>>> Handle(GetPostsCommand request, CancellationToken cancellationToken)
        {
            var posts = await _context.Posts
                .Select(p => new GetPostsResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CondominiumId = p.CondominiumId,
                    UserId = p.UserId,
                    LevelOfPriorityId = p.LevelOfPriorityId
                })
                .ToListAsync(cancellationToken);

            return Result.Ok(posts);
        }
    }
}