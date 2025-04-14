using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Context;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories
{
    internal class CommentRepository
    : Repository<Comment, Guid>, ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}