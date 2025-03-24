
using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Delete
{
    public class DeletePriorityLevelCommand : ICommand<Result<DeletePriorityLevelResponse>>
    {
        public Guid Id { get; set; }
        public Guid CondominiumId { get; set; }
    }
}
