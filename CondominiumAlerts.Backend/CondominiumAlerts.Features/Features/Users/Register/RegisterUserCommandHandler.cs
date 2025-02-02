
using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using LightResults;

namespace CondominiumAlerts.Features.Commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<User>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IRepository<User, string> _userRepository;
    
    public RegisterUserCommandHandler(IAuthenticationProvider authenticationProvider, IRepository<User, string> userRepository)
    {
        _authenticationProvider = authenticationProvider;
        _userRepository = userRepository;
    }
    public async Task<Result<User>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        string? identityId = null;
    
        try
        {
            // 📌 Registrar usuario en Firebase
            identityId = await _authenticationProvider.RegisterUserAsync(request.Username, request.Email, request.Password, cancellationToken);
            //Si falla el registro en Firebase, no seguir el proceso
            if (string.IsNullOrEmpty(identityId))
            {
                return Result.Fail<User>("Error al registrar el usuario en Firebase.");
            }

            // 📌 Crear el usuario en la base de datos
            var user = new User
            {
                Id = identityId, // Relacionar Firebase con la BD
                Name = request.Username,
                LastName = request.Lastname,
                Phone = request.PhoneNumber,
                Email = request.Email,
            };

            user = await _userRepository.CreateAsync(user, cancellationToken);
            
            return Result.Ok<User>(user);
        }
        catch (FirebaseAuthException ex)
        {
            // Comparar el ErrorCode con el string "email-already-exists"
           if (ex.ErrorCode == ErrorCode.AlreadyExists)
            {
                return Result.Fail<User>("El correo electrónico ya está registrado en Firebase.");
            }

            // Si es otro error de Firebase, devolver un mensaje genérico
            return Result.Fail<User>($"Error al registrar el usuario en Firebase: {ex.Message}");
        }
        catch (Exception ex)
        {
            // 📌 Si la BD falla, eliminar el usuario de Firebase
            if (!string.IsNullOrEmpty(identityId))
            {
                await _authenticationProvider.DeleteUserAsync(identityId, cancellationToken);
                await _userRepository.DeleteAsync(identityId, cancellationToken);
            }

            return Result.Fail<User>($"Error durante el registro: {ex.Message}");
        }
    }
}