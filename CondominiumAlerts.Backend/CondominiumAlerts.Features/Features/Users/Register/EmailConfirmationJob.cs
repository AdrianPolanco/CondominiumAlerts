
using CondominiumAlerts.Infrastructure.Auth.Interfaces;
using Coravel.Invocable;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Users.Register;

public class EmailConfirmationJob : IInvocable, IInvocableWithPayload<RegisterUserResponse>, ICancellableInvocable
{

    private readonly ILogger<EmailConfirmationJob> _logger;
    private readonly IEmailService _emailService;
    public RegisterUserResponse Payload { get; set; }
    public CancellationToken CancellationToken { get; set; }
    
    public EmailConfirmationJob(ILogger<EmailConfirmationJob> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    
    public async Task Invoke()
    {
        var uniqueId = Guid.NewGuid().ToString();

        if (CancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning($"[{uniqueId}] La tarea fue cancelada antes de comenzar.");
            return;
        }

        try
        {
            _logger.LogInformation($"[{uniqueId}] Generando link de verificación de cuenta para {Payload.Email}");
            
            var verificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(Payload.Email);

            _logger.LogInformation($"[{uniqueId}] Enlace de verificación generado para {Payload.Email}");

            if (CancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning($"[{uniqueId}] La tarea fue cancelada antes de enviar el correo.");
                return;
            }

            _logger.LogInformation($"[{uniqueId}] Enviando correo de verificación a {Payload.Email}");

            await _emailService.SendConfirmationEmailAsync(Payload.Name, Payload.LastName, Payload.Email, verificationLink, CancellationToken);

            _logger.LogInformation($"[{uniqueId}] Correo de verificación enviado exitosamente a {Payload.Email}");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"[{uniqueId}] La operación fue cancelada.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{uniqueId}] Error al enviar el correo de verificación: {ex.Message}");
        }
    }
    
}