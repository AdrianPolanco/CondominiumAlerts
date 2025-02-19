using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Persistence.Context;

namespace CondominiumAlerts.Infrastructure.Persistence.Repositories
{
    internal class CondominiumRepository
    : Repository<Condominium, Guid>, ICondominiumRepository
    {
        private readonly ApplicationDbContext _context;

        public CondominiumRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}