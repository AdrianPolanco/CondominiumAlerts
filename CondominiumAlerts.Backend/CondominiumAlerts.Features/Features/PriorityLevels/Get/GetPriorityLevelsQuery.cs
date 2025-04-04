using LightResults;
using MediatR;

namespace CondominiumAlerts.Features.Features.PriorityLevels.Get
{
    public class GetPriorityLevelsQuery : IRequest<Result<GetPriorityLevelResponce>>
    {
        public Guid CondominiumId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
