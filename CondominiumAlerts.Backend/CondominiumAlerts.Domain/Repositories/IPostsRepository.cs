using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Domain.Repositories
{
    public interface IPostsRepository : IRepository<Post, Guid>
    {

    }
}
