
using LightResults;
using MediatR;

namespace CondominiumAlerts.Features.Features.PriorityLevels.GetById
{
    public class GetByIdPriorityLevelQuery : IRequest<Result<GetByIdPriorityLevelResponse>>
    {
        public Guid Id { get; set; }
        public Guid CondominiumId { get; set; }
    }
}
