using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Events.Interfaces;

namespace CondominiumAlerts.Features.Events.Users;

public class CreateUserEventHandler : IEventHandler<CreateUserEvent>
{
    public Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}