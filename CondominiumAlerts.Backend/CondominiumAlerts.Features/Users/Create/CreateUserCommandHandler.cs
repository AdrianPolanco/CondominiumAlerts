using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using MediatR;

namespace CondominiumAlerts.Features.Commands;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}