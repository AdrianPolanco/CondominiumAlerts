using CondominiumAlerts.Infrastructure.Persistence.Context;
using LightResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CondominiumAlerts.Features.Features.Posts.Get
{
    public class GetPostByIdHandler : IRequestHandler<GetPostByIdCommand, Result<GetPostByIdResponse>>
    {
        private readonly ApplicationDbContext _context;

        public GetPostByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<GetPostByIdResponse>> Handle(GetPostByIdCommand request, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Where(p => p.Id == request.Id)
                .Select(p => new GetPostByIdResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (post == null)
                return Result.Fail<GetPostByIdResponse>("No se encontró el post con el ID especificado.");

            return Result.Ok(post);
        }
    }
}