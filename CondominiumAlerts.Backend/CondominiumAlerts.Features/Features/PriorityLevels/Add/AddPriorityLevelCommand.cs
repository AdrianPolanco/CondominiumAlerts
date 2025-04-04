
using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;


namespace CondominiumAlerts.Features.Features.PriorityLevels.Add
{
    public class AddPriorityLevelCommand : ICommand<Result<AddPriorityLevelResponse>>
    {
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CondominiumId { get; set; }
    }
}
