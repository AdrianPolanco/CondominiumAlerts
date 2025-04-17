using CondominiumAlerts.Features.Features.Comment.GetCommentByPost;
using CondominiumAlerts.Infrastructure.Persistence.Context;
using LightResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CondominiumAlerts.Features.Features.Posts.Get
{
    public class GetCommentByPostHandler : IRequestHandler<GetCommentByPostCommand, Result<List<GetCommentByPostResponse>>>
    {
        private readonly ApplicationDbContext _dbContext; 

        public GetCommentByPostHandler(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext; 
        }

        public async Task<Result<List<GetCommentByPostResponse>>> Handle(GetCommentByPostCommand request, CancellationToken cancellationToken)
        {
            var comment = await _dbContext.Comments 
                .Where(c => c.PostId == request.PostId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new GetCommentByPostResponse
                {
                    Id = p.Id,
                    Text = p.Text,
                    ImageUrl = p.ImageUrl ?? string.Empty,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    UserId = p.UserId,
                    UserProfilePicture = p.User.ProfilePictureUrl,
                    Username = p.User.Username,
                    PostId = p.PostId

                })
                .ToListAsync(cancellationToken);

            if (!comment.Any())
                return Result.Fail<List<GetCommentByPostResponse>>("No comment found for the given post ID.");

            return Result.Ok(comment);
        }
    }
}