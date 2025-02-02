
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<object>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IRepository<User, string> _userRepository;
    
    public RegisterUserCommandHandler(IAuthenticationProvider authenticationProvider, IRepository<User, string> userRepository)
    {
        _authenticationProvider = authenticationProvider;
        _userRepository = userRepository;
    }
    public async Task<Result<object>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        string? identityId = null;
    
        try
        {
            // 📌 Registrar usuario en Firebase
            identityId = await _authenticationProvider.RegisterUserAsync(request.Username, request.Email, request.Password, cancellationToken);
            //Si falla el registro en Firebase, no seguir el proceso
            if (string.IsNullOrEmpty(identityId))
            {
                return Result.Fail<object>("Error al registrar el usuario en Firebase.");
            }

            // 📌 Crear el usuario en la base de datos
            var user = new User
            {
                Id = identityId, // Relacionar Firebase con la BD
                Name = request.Username,
                LastName = request.Lastname,
                Address = request.Address,
                Phone = request.PhoneNumber
            };

            user = await _userRepository.CreateAsync(user, cancellationToken);

            return Result.Ok(user);
        }
        catch (Exception ex)
        {
            // 📌 Si la BD falla, eliminar el usuario de Firebase
            if (!string.IsNullOrEmpty(identityId))
            {
                await _authenticationProvider.DeleteUserAsync(identityId, cancellationToken);
            }

            return Result.Fail<object>($"Error durante el registro: {ex.Message}");
        }
    }
}