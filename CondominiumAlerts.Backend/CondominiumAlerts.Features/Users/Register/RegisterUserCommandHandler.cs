using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.CrossCutting.Results;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;

namespace CondominiumAlerts.Features.Commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, CustomResult<object>>
{
    
    private readonly IAuthenticationProvider _authenticationProvider;

    public RegisterUserCommandHandler(IAuthenticationProvider authenticationProvider)
    {
        _authenticationProvider = authenticationProvider;
    }
    public async Task<CustomResult<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var identityId = await _authenticationProvider.RegisterUserAsync(request.Username, request.Email, request.Password, cancellationToken);
        return CustomResult<object>.Success(new { IdentityId = identityId, Username = request.Username, Email = request.Email });
    }
}