using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;
using Mapster;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Users.Update;

public class BasicUpdateUserStrategy : IUpdateUserStrategy
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly ILogger<BasicUpdateUserStrategy> _logger;
    private readonly IRepository<User, string> _repository;
    
    public BasicUpdateUserStrategy(
        IAuthenticationProvider authenticationProvider, 
        ILogger<BasicUpdateUserStrategy> logger, 
        IRepository<User, string> userRepository)
    {
        _authenticationProvider = authenticationProvider;
        _logger = logger;
        _repository = userRepository;
    }
    public bool CanHandle(UpdateUserCommand input)
    {
        var canHandle = input.ProfilePic is null;
        _logger.LogInformation($"BasicUpdateUserStrategy.CanHandle: {canHandle}");
        return canHandle;
    }

    public async Task<Result<UpdateUserResponse>> HandleAsync(UpdateUserCommand input, CancellationToken cancellationToken)
    {
        var doesUserExist = await _authenticationProvider.DoesUserExistAsync(input.Id, cancellationToken);
        
        if (!doesUserExist) return Result<UpdateUserResponse>.Fail($"El usuario con el Id {input.Id} no existe");
        
        var result = await _authenticationProvider.UpdateUserAsync(input.Id, input.Username, cancellationToken);

        if (!result) return Result<UpdateUserResponse>.Fail("No se pudo actualizar los datos del usuario");
        
        User user = await _repository.GetByIdAsync(input.Id, cancellationToken);
        
        user.Name = input.Name;
        user.Lastname = input.Lastname;
        user.Address = input.Address;
        user.Username = input.Username;
        
        user = await _repository.UpdateAsync(user, cancellationToken)!;

        var response = user.Adapt<UpdateUserResponse>();
        
        return Result<UpdateUserResponse>.Ok(response);

    }
}