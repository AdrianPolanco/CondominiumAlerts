using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Context;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories
{
    internal class PostsRepository
    : Repository<Post, Guid>, IPostsRepository
    {
        private readonly ApplicationDbContext _context;

        public PostsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}