
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<object>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    
    public RegisterUserCommandHandler(IAuthenticationProvider authenticationProvider)
    {
        _authenticationProvider = authenticationProvider;
    }
    public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var identityId = await _authenticationProvider.RegisterUserAsync(request.Username, request.Email, request.Password, cancellationToken);
        return Result.Ok<object>(new
        {
            IdentityId = identityId,
            Username = request.Username,
            Email = request.Email
        });
    }
}