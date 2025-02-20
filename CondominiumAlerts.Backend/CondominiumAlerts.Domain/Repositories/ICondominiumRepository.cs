using CondominiumAlerts.Domain.Aggregates.Entities;

namespace CondominiumAlerts.Domain.Repositories
{
    public interface ICondominiumRepository : IRepository<Condominium, Guid>
    {

    }
}