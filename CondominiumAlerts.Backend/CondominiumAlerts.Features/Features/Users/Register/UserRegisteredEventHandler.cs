using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Users.Register;

public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;
    private readonly IEmailService _emailService;
    
    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var uniqueId = Guid.NewGuid().ToString();

        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning($"[{uniqueId}] La tarea fue cancelada antes de comenzar.");
            return;
        }

        try
        {
            _logger.LogInformation($"[{uniqueId}] Generando link de verificación de cuenta para {notification.Email}.", uniqueId, notification.Email);
            
            var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(notification.Email);

            _logger.LogInformation($"[{uniqueId}] Enlace de verificación generado para {notification.Email}", uniqueId, notification.Email);

            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[{uniqueId}] La tarea fue cancelada antes de enviar el correo.");
                return;
            }

            _logger.LogInformation($"[{uniqueId}] Enviando correo de verificación a {notification.Email}", uniqueId, notification.Email);

            await _emailService.SendConfirmationEmailAsync(notification.Name, notification.LastName, notification.Email, verificationLink, cancellationToken);

            _logger.LogInformation($"[{uniqueId}] Correo de verificación enviado exitosamente a {notification.Email}", uniqueId, notification.Email);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"[{uniqueId}] La operación fue cancelada.", uniqueId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{uniqueId}] Error al enviar el correo de verificación: {ex.Message}", uniqueId);
        }
    }
}