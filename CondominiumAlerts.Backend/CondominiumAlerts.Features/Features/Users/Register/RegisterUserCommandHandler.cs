using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using Coravel.Queuing.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FluentValidation;
using LightResults;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Users.Register;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IRepository<User, string> _userRepository;
    private readonly IQueue _queue;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly IValidator<RegisterUserCommand> _validator;
    public RegisterUserCommandHandler(IAuthenticationProvider authenticationProvider, IRepository<User, string> userRepository, IQueue queue, ILogger<RegisterUserCommandHandler> logger, IValidator<RegisterUserCommand> validator)
    {
        _authenticationProvider = authenticationProvider;
        _userRepository = userRepository;
        _queue = queue;
        _logger = logger;
        _validator = validator;
    }
    public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResults = await _validator.ValidateAsync(request);

        if (!validationResults.IsValid)
        {
            // Crear un mensaje de error a partir de los resultados de la validación
            var validationErrors = validationResults.Errors
                .Select(e => e.ErrorMessage)
                .ToList();
            _logger.LogWarning($"Validation failed: {validationErrors}");

            return Result.Fail<RegisterUserResponse>(string.Join(", ", validationErrors));
        }
        
        string? identityId = null;
    
        try
        {
            // 📌 Registrar usuario en Firebase
            identityId = await _authenticationProvider.RegisterUserAsync(request.Username, request.Email, request.Password, cancellationToken);
            //Si falla el registro en Firebase, no seguir el proceso
            if (string.IsNullOrEmpty(identityId))
            {
                return Result.Fail<RegisterUserResponse>("Error al registrar el usuario en Firebase.");
            }

            // 📌 Crear el usuario en la base de datos
            var user = new User
            {
                Id = identityId, // Relacionar Firebase con la BD
                Name = request.Username,
                Lastname = request.Lastname,
                Email = request.Email,
            };

            await _userRepository.CreateAsync(user, cancellationToken);

            var registerUserResponse = user.Adapt<RegisterUserResponse>();

            _logger.LogInformation($"Encolando EmailConfirmationJob para {registerUserResponse.Email}");
            _queue.QueueInvocableWithPayload<EmailConfirmationJob, RegisterUserResponse>(registerUserResponse);
            return Result.Ok<RegisterUserResponse>(registerUserResponse);
        }
        catch (FirebaseAuthException ex)
        {
            // Comparar el ErrorCode con el string "email-already-exists"
           if (ex.ErrorCode == ErrorCode.AlreadyExists)
            {
                return Result.Fail<RegisterUserResponse>("El correo electrónico ya está registrado en Firebase.");
            }

            // Si es otro error de Firebase, devolver un mensaje genérico
            return Result.Fail<RegisterUserResponse>($"Error al registrar el usuario en Firebase: {ex.Message}");
        }
        catch (Exception ex)
        {
            // 📌 Si la BD falla, eliminar el usuario de Firebase
            if (!string.IsNullOrEmpty(identityId))
            {
                await _authenticationProvider.DeleteUserAsync(identityId, cancellationToken);
                await _userRepository.DeleteAsync(identityId, cancellationToken);
            }

            return Result.Fail<RegisterUserResponse>($"Error durante el registro: {ex.Message}");
        }
    }
}