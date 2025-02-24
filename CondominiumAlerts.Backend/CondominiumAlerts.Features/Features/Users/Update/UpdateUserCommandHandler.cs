using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Interfaces;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Users.Update;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    private readonly IValidator<UpdateUserCommand> _validator;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IUpdateUserStrategy _updateUserStrategy;

    public UpdateUserCommandHandler(
        IValidator<UpdateUserCommand> validator, 
        ILogger<UpdateUserCommandHandler> logger,
        IUpdateUserStrategy updateUserStrategy
        )
    {
        _validator = validator;
        _logger = logger;
        _updateUserStrategy = updateUserStrategy;
    }
    
    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            IEnumerable<string> errors = validationResult.Errors.Select(e => e.ErrorMessage);
            _logger.LogTrace($"Validation failed {errors}");
            return Result.Fail<UpdateUserResponse>(string.Join(", ", errors));
        }
        
        return await _updateUserStrategy.HandleAsync(request, cancellationToken);
    }
}