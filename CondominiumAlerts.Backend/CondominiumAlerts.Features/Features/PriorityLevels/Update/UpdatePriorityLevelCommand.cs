

using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Update
{
    public class UpdatePriorityLevelCommand : ICommand<Result<UpdatePriorityLevelResponse>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
