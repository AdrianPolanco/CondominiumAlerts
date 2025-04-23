
using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Posts.Delete
{
    public class DeletePostCommand : ICommand<Result<DeletePostResponse>>
    {
        public Guid Id { get; set; }
        public Guid CondominiumId { get; set; }
    }
}
