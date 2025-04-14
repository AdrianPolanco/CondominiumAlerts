using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Domain.Repositories
{
    public interface ICommentRepository : IRepository<Comment, Guid>
    {

    }
}
